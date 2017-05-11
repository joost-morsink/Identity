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
            Assert.IsTrue(idprov.SupportsNewIdentities);
            Assert.AreNotEqual(idprov.New(new A()), idprov.New(new A()));
            var a = new A();
            Assert.AreNotEqual(idprov.New(a), idprov.New(a));
            Assert.AreEqual(idprov.New(typeof(A), new A()).ForType, typeof(A));
        }
        [TestMethod]
        public void Generator_CodeId()
        {
            var d = new D { Code = "Test" };
            Assert.IsNull(d.Id);
            d.Id = idprov.NewDId(d);
            Assert.AreEqual(idprov, d.Id.Provider);
            Assert.AreEqual("Test", d.Id.Value);
            Assert.IsTrue(idprov.Equals(idprov.NewDId(d), idprov.NewDId(d)));
        }
    }
}
