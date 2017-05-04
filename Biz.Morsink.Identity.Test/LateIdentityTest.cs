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
        public void LateId()
        {
            var gip = new GenericIdentityProvider<int>();
            var lid = new LateIdentity<A, int>(gip);
            Assert.IsFalse(lid.IsAvailable, "Unresolved late identity should not be available");
            Assert.IsFalse(gip.Equals(lid, gip.Creator<A>().Create(42)), "Unresolved late identity should not equal another identity");
            Assert.IsTrue(gip.Equals(lid, lid), "Unresolved late identity should equal itself");
            Assert.IsFalse(gip.Equals(lid, new LateIdentity<A, int>(gip)), "Unresolved late identity should not equal another unresolved late identity");
            lid.Resolve(42);
            Assert.IsTrue(lid.IsAvailable, "Resolved late identity should be available");
            Assert.AreEqual(42, lid.Value, "Resolved late identity's value should equal the value the late identity has been resolved with");
            Assert.IsTrue(gip.Equals(lid, gip.Creator<A>().Create(42)), "Resolved late identity should equal a regular identity with the same type and value");
        }
    }
}
