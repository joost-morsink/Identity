using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class SystemIdentityTest
    {
        private TestIdProvider provider;

        [TestInitialize]
        public void Init()
        {
            provider = TestIdProvider.Instance;
        }

        [TestMethod]
        public void SystemId_Happy()
        {
            var id = FreeIdentity<Person>.Create(42);
            var decId = id.WithSystem(new SystemIdentity("SYS"));

            Assert.AreEqual(typeof(Person), decId.ForType);
            Assert.AreEqual("SYS", decId.For<System>().Value);
            Assert.AreEqual(42, decId.For<Person>().Value);
            Assert.AreEqual(1, decId.Arity);
        }

        [TestMethod]
        public void SystemId_MapHappy()
        {
            var id = FreeIdentity<Person>.Create("42");
            var decId = id.WithSystem(new SystemIdentity("SYS"));
            var pid = provider.Translate(decId);

            Assert.AreEqual(typeof(Person), pid.ForType);
            Assert.AreEqual("SYS", pid.For<System>().Value);
            Assert.AreEqual(42, pid.For<Person>().Value);
            Assert.AreEqual(1, pid.Arity);
        }
    }
}
