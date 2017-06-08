using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class GeneratorTest
    {
        private TestIdProvider idprov;

        [TestInitialize]
        public void Init()
        {
            idprov = new TestIdProvider();
        }
        [TestMethod]
        public void Generator_Happy()
        {
            Assert.IsTrue(idprov.SupportsNewIdentities, "ReflectedIdentityProviders with [Generator] attributed methods should support new identities.");
            Assert.AreNotEqual(idprov.New(new A()), idprov.New(new A()),"Two generated identity values on different objects should not equal each other.");
            var a = new A();
            Assert.AreNotEqual(idprov.New(a), idprov.New(a),"Two generated identity values on the same object should not equal each other.");
            Assert.AreEqual(idprov.New(typeof(A), new A()).ForType, typeof(A), "A generated identity value should have the ForType set to the type it was generated for.");
            Assert.IsFalse(idprov.Equals(idprov.NewAId(), idprov.NewAId()), "Two generated identity values should not equal each other.");
            var aid = idprov.NewAId();
            Assert.AreEqual(aid, aid, "A generated identity value should equal itself (reflexive property).");
        }
        [TestMethod]
        public void Generator_CodeId()
        {
            var d = new D { Code = "Test" };
            Assert.IsNull(d.Id, "Identity property should not be preset.");
            d.Id = idprov.NewDId(d);
            Assert.AreEqual(idprov, d.Id.Provider, "A generated identity value should reference its generating identity provider.");
            Assert.AreEqual("Test", d.Id.Value, "A generator that is based on a data property should use that value as the underlying identity value.");
            Assert.IsTrue(idprov.Equals(idprov.NewDId(d), idprov.NewDId(d)), "A generator that is based on a data property might produce equal values.");
        }
    }
}
