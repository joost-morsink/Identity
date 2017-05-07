using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class GeneratorTest
    {
        [TestMethod]
        public void Generator_Happy()
        {
            var idprov = new TestIdProvider();
            Assert.IsTrue(idprov.SupportsNewIdentities);
            Assert.AreNotEqual(idprov.New(new A()), idprov.New(new A()));
            var a = new A();
            Assert.AreNotEqual(idprov.New(a), idprov.New(a));
            Assert.AreEqual(idprov.New(typeof(A), new A()).ForType, typeof(A));
        }
    }
}
