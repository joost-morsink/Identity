using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Biz.Morsink.DataConvert;
namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestMethod1()
        {
            var p = new Person { Id = IdProvider.Instance.Create<Person, int>(1), Name = "Joost", Age = 37 };
            var d = new Detail { Id = IdProvider.Instance.Create<Detail, (int, int)>((1, 2)).Upgrade(), Description = "Detail" };
            Assert.AreEqual(1, p.Id.ComponentValue);
            Assert.AreEqual(2, d.Id.ComponentValue);
            Assert.AreEqual(1, d.Id.Identities.OfType<IIdentity<Person>>().First().ComponentValue);
            Assert.IsTrue(IdProvider.Instance.Equals(p.Id, d.Id.Identities.OfType<IIdentity<Person>>().First()));
            Assert.IsTrue(IdProvider.Instance.Equals(d.Id, IdProvider.Instance.Create<Detail, (int, int)>((1, 2))));
        }
    }
    public class IdProvider : AbstractIdentityProvider
    {
        public static IdProvider Instance { get; } = new IdProvider();



        public IIdentity<Person> PersonId<K>(K value)
        {
            return this.Builder<Person>().Create(DataConverter.Default.Convert(value).To<int>());
        }
        public IIdentity<Detail> DetailId<K>(K value)
        {
            var (p, d) = DataConverter.Default.Convert(value).To<(int, int)>();
            return new Identity<Person, Detail, int, int>(this, p, d);
        }
    }

    public class Person
    {
        public IIdentity<Person> Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    public class Detail
    {
        public IIdentity<Person, Detail> Id { get; set; }
        public string Description { get; set; }
    }
    public static class DomainExt
    {
        public static IIdentity<Person, Detail> Upgrade(this IIdentity<Detail> id)
        {
            return id as IIdentity<Person, Detail> ?? id.Provider.Translate(id) as IIdentity<Person, Detail>;
        }
    }
}
