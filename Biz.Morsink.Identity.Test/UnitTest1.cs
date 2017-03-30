using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Biz.Morsink.DataConvert;
namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class UnitTest1
    {
        class IdProvider : AbstractIdentityProvider
        {
            public static IdProvider Instance { get; } = new IdProvider();


            public override IIdentity Translate(IIdentity id)
            {
                if (id.ForType == typeof(Person))
                    return new Identity<Person, int>(this, GetConverter(typeof(Person), true).Convert(id.Value).To<int>());
                else
                    return null;
            }
            
        }

        class Person
        {
            public IIdentity<Person> Id { get; set; }
            public string Name { get; set; }
            public int Age { get; set; }
        }
        [TestMethod]
        public void TestMethod1()
        {
            var p = new Person { Id = IdProvider.Instance.Builder<Person>().Create(1), Name = "Joost", Age = 37 };

        }
    }
}
