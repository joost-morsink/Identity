using Biz.Morsink.Identity.Decoration;
using Biz.Morsink.Identity.Systems;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class DecoratedIdentityTest
    {
        private TestIdProvider provider;

        [TestInitialize]
        public void Init()
        {
            provider = TestIdProvider.Instance;
        }
        [TestMethod]
        public void DecoratedId_Happy()
        {
            var id = FreeIdentity<Person>.Create(42);
            var decId = id.Decorate(new SystemIdentity("SYS"));

            Assert.AreEqual(typeof(Person), decId.ForType, "Decoration should preserve the ForType property." );
            Assert.AreEqual("SYS", decId.For<Sys>().Value, "The decoration should be available in the Identities property.");
            Assert.AreEqual(42, decId.For<Person>().Value, "The original identity value should be preserved in the decorated value.");
            Assert.AreEqual(1, decId.Arity, "Decoration should preserve the Arity property.");
        }

        [TestMethod]
        public void DecoratedId_MapHappy()
        {
            var id = FreeIdentity<Person>.Create("42");
            var decId = id.Decorate(new SystemIdentity("SYS"));
            var pid = provider.Translate(decId);

            Assert.AreEqual(typeof(Person), pid.ForType, "Translation after decoration should preserve the ForType property.");
            Assert.AreEqual("SYS", pid.For<Sys>().Value, "Translation should preserve the decoration.");
            Assert.AreEqual(42, pid.For<Person>().Value, "Translation after decoration should preserve the underlying Value.");
            Assert.AreEqual(1, pid.Arity,"Translation after decoration should preserve the Arity property.");
        }
    }
}
