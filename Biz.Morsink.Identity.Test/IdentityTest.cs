using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Biz.Morsink.DataConvert;
namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class IdentityTest
    {
        private Person person;
        private Detail detail;
        private IdProvider provider;
        [TestInitialize]
        public void Init()
        {
            provider = IdProvider.Instance;
            person = new Person { Id = provider.Creator<Person>().Create("1"), Name = "Joost", Age = 37 };
            detail = new Detail { Id = provider.Creator<Detail>().Create(("1", "2")).Upgrade(), Description = "Detail" };
        }
        [TestMethod]
        public void Identity_ComponentValues()
        {
            Assert.AreEqual(1, person.Id.ComponentValue, "Component value should be equal to the converted value");
            Assert.AreEqual(2, detail.Id.ComponentValue, "Component value should be equal to the converted component value");
        }
        [TestMethod]
        public void Identity_Parents()
        {
            Assert.AreEqual(1, detail.Id.Parent.ComponentValue, "Parent's component value should be equal to the converted component value");
            Assert.AreEqual(1, detail.Id.For<Person>()?.ComponentValue, "Person Identity must be found and have a component value equal to the converted component value");
        }
        [TestMethod]
        public void Identity_Equality()
        {
            Assert.IsTrue(provider.Equals(person.Id, detail.Id.For<Person>()), "Intrinsic Person identity should be equal to the Person identity");
            Assert.IsTrue(provider.Equals(detail.Id, provider.Create<Detail, (int, int)>((1, 2))), "Full identity should be equal tot the converted identity using generic method");
            Assert.IsTrue(provider.Equals(detail.Id, provider.DetailId(1, 2)), "Full identity should be equal tot the converted identity using specific method");
            Assert.IsTrue(provider.Equals(provider.DId(null), provider.DId(null)), "Null identity values should be equal");
            Assert.IsTrue(provider.Equals(null, null), "Null identities should be equal");
        }
        [TestMethod]
        public void Identity_Arity()
        {
            Assert.AreEqual(2, detail.Id.Arity, "Identity<T,U> should have arity 2");
            Assert.AreEqual(1, person.Id.Arity, "Identity<T> should have arity 1");
        }
        [TestMethod]
        public void Identity_Translation()
        {
            var aid = new Identity<A, string>(null, "42");
            Assert.AreEqual(typeof(int), provider.Translate(aid).ComponentValue.GetType(), "A different-typed identity should be of the provider's type after translation");
            Assert.AreEqual(42, provider.Translate(aid).ComponentValue, "A different-typed identity should be converted during translation");
        }
        [TestMethod]
        public void Identity_CreateFailures()
        {
            Assert.IsNull(provider.Creator<A>().Create("x"), "A failed conversion should yield null as identity value");
            Assert.IsNull(provider.Creator<B>().Create("42"), "An absent conversion method should yield null as identity value");
        }
        [TestMethod]
        public void Identity_Typing()
        {
            Assert.AreEqual(typeof(A), provider.Creator(typeof(A)).Create("42").ForType, "General creation of identities should respect the ForType property");
            Assert.IsNotNull(provider.Creator(typeof(A)).Create("42") as IIdentity<A>, "General creation of identities should yield a strongly typed runtime value");
            Assert.AreEqual(42, provider.Creator(typeof(A)).Create("42").Value, "General creation of identities should convert value according to the type");
        }
        [TestMethod]
        public void Identity_GenericCreator()
        {
            var CId = provider.Creator<C>();
            Assert.AreEqual(42, CId.Create(42)?.ComponentValue, "Generic creation method should respect the integer type");
            Assert.AreEqual(42L, CId.Create(42L)?.ComponentValue, "Generic creation method should respect the long type");
            Assert.AreEqual(42, CId.Create("42")?.ComponentValue, "Generic creation method should convert to int");
            Assert.AreNotEqual(42L, CId.Create("42")?.ComponentValue, "Generic creation method should not convert to long");
        }
    }
    public class IdProvider : ReflectedIdentityProvider
    {
        public static IdProvider Instance { get; } = new IdProvider();

        public Identity<Person, int> PersonId(int value)
            => new Identity<Person, int>(this, value);

        public Identity<Person, Detail, int, int> DetailId(int p, int d)
            => new Identity<Person, Detail, int, int>(this, p, d);

        public IIdentity<A> AId(int x)
            => new Identity<A, int>(this, x);

        // This method definition does not satisfy the constraints for identity value creation and will not be used by the generic mechanism.
        public IIdentity BId(int x)
            => new Identity<B, int>(this, x);

        public IIdentity<C> CId<K>(K value)
        {
            if (typeof(K) == typeof(int) || typeof(K) == typeof(long))
                return new Identity<C, K>(this, value);
            else if (GetConverter(typeof(C), true).Convert(value).TryTo(out int v))
                return new Identity<C, int>(this, v);
            else
                return null;
        }
        public IIdentity<D> DId(string value)
            => new Identity<D, string>(this, value);
    }
}
