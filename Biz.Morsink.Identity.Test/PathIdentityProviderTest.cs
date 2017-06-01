using Biz.Morsink.Identity.PathProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class PathIdentityProviderTest
    {
        private TestPathIdentityProvider pp;

        [TestInitialize]
        public void Init()
        {
            pp = TestPathIdentityProvider.Instance;
        }

        [TestMethod]
        public void PathIdProv_HappyCreateUnary()
        {
            var pid = pp.Creator<Person>().Create("123");
            Assert.IsNotNull(pid);
            Assert.AreEqual("123", pid.Value);
        }
        [TestMethod]
        public void PathIdProv_HappyCreateBinary() {
            var did = pp.Creator<Detail>().Create(("123", 1));
            Assert.IsNotNull(did);
            Assert.AreEqual(("123", "1"), did.Value);
        }
        [TestMethod]
        public void PathIdProv_HappyParse() {
            var tid = pp.Parse("/api/person/123/detail/45");
            Assert.AreEqual(typeof(Detail), tid.ForType);
            Assert.AreEqual(("123", "45"), tid.Value);
            Assert.AreEqual("45", tid.ComponentValue);
            Assert.IsNotNull(tid.For<Person>());
            Assert.AreEqual("123", tid.For<Person>().ComponentValue);
        }
        [TestMethod]
        public void PathIdProv_HappyToPath() {
            var pid = pp.Creator<Person>().Create(123);
            var did = pp.Creator<Detail>().Create((123, 1));
            Assert.AreEqual("/api/person/123", pp.ToPath(pid));
            Assert.AreEqual("/api/person/123/detail/1", pp.ToPath(did));
        }
        [TestMethod]
        public void PathIdProv_HappyUnknown() 
        {
            var up = pp.Parse("/unknown/path/123", false);
            Assert.IsNotNull(up);
            Assert.AreEqual(typeof(object), up.ForType);
            Assert.AreEqual("/unknown/path/123", up.Value.ToString());
        }
        [TestMethod]
        public void PathIdProv_ArityMismatch()
        {
            Assert.ThrowsException<ArgumentException>(() => new ArityMismatchProvider());
        }

        public class ArityMismatchProvider : PathIdentityProvider
        {
            public ArityMismatchProvider()
            {
                AddEntry("/api/a/*/*", typeof(A));
            }
        }
        public class TestPathIdentityProvider : PathIdentityProvider
        {
            public static TestPathIdentityProvider Instance { get; } = new TestPathIdentityProvider();
            public TestPathIdentityProvider()
            {
                AddEntry("/api/person/*", typeof(Person));
                AddEntry("/api/person/*/detail/*", typeof(Person), typeof(Detail));
                AddEntry("/api/person/*/detail/*/*", typeof(Person), typeof(Detail), typeof(Sub));
            }
        }
    }
}

