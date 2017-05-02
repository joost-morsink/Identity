using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class LateIdentityTest 
    {
        [TestMethod]
        public void LateId_Happy()
        {
            var gip = new GenericIdentityProvider<int>();
            var lid = new LateIdentity<A, int>(gip);
            Assert.IsFalse(lid.IsAvailable);
            Assert.IsFalse(gip.Equals(lid, gip.Creator<A>().Create(42)));
            Assert.IsTrue(gip.Equals(lid, lid));
            Assert.IsFalse(gip.Equals(lid, new LateIdentity<A, int>(gip)));
            lid.Resolve(42);
            Assert.IsTrue(lid.IsAvailable);
            Assert.AreEqual(42, lid.Value);
            Assert.IsTrue(gip.Equals(lid, gip.Creator<A>().Create(42)));
        }
    }
}
