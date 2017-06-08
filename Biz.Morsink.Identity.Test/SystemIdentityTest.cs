using Biz.Morsink.Identity.Systems;
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

            Assert.AreEqual(typeof(Person), decId.ForType, "Adding a system should preserve the ForType property.");
            Assert.AreEqual("SYS", decId.For<Sys>().Value, "Adding a system should allow the system to be found.");
            Assert.AreEqual(42, decId.For<Person>().Value, "Adding a system should preserve the InnerIdentity's underlying value.");
            Assert.AreEqual(1, decId.Arity, "Adding a system should preserve the Arity property.");
        }

        [TestMethod]
        public void SystemId_MapHappy()
        {
            var id = FreeIdentity<Person>.Create("42");
            var decId = id.WithSystem(new SystemIdentity("SYS"));
            var pid = provider.Translate(decId);

            Assert.AreEqual(typeof(Person), pid.ForType, "Translating an identity value with system should preserve the ForType property.");
            Assert.AreEqual("SYS", pid.For<Sys>().Value, "Translating an identity value with system should preserve the system identity value.");
            Assert.AreEqual(42, pid.For<Person>().Value, "Translating an identity value with system should preserve the InnerIdentity's underlying value.");
            Assert.AreEqual(1, pid.Arity, "Translating an identity value with system should preserve the Arity property.");
        }
    }
}
