using Biz.Morsink.Identity.PathProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class PathIdentityProviderTest
    {
        private TestPathIdentityProvider pp;
        private TestPathIdentityProvider cipp;

        [TestInitialize]
        public void Init()
        {
            pp = TestPathIdentityProvider.CaseSensitive;
            cipp = TestPathIdentityProvider.CaseInsensitive;
        }

        [TestMethod]
        public void PathIdProv_HappyCreateUnary()
        {
            var pid = pp.Creator<Person>().Create("123");
            Assert.IsNotNull(pid, "PathIdentityProvider should allow creation of known unary identity values.");
            Assert.AreEqual("123", pid.Value, "PathIdentityProvider should preserve the underlying value in the creation of unary identity values.");
        }
        [TestMethod]
        public void PathIdProv_HappyCreateBinary()
        {
            var did = pp.Creator<Detail>().Create(("123", 1));
            Assert.IsNotNull(did, "PathIdentityProvider should allow creation of known binary identity values.");
            Assert.AreEqual(("123", "1"), did.Value, "PathIdentityProvider should preserve the underlying value in the creation of binary identity values.");
        }
        [TestMethod]
        public void PathIdProv_HappyParse()
        {
            var tid = pp.Parse("/api/person/123/detail/45");
            Assert.AreEqual(typeof(Detail), tid.ForType, "PathIdentityProvider should be able to parse 2-ary paths.");
            Assert.AreEqual(("123", "45"), tid.Value, "The path parts on wildcard positions should be preserved in the underlying identity value.");
            Assert.AreEqual("45", tid.ComponentValue, "The component value should equal the last wildcard.");
            Assert.IsNotNull(tid.For<Person>(), "Parent identities should be preserved in n-ary paths.");
            Assert.AreEqual("123", tid.For<Person>().ComponentValue, "Parent identities' component value should be preserved in n-ary paths.");
        }
        [TestMethod]
        public void PathIdProv_HappyParseCaseInsensitive()
        {
            var tid = cipp.Parse("/API/Person/123/deTAIl/45");
            Assert.AreEqual(typeof(Detail), tid.ForType, "PathIdentityProvider should be able to parse 2-ary paths.");
            Assert.AreEqual(("123", "45"), tid.Value, "The path parts on wildcard positions should be preserved in the underlying identity value.");
            Assert.AreEqual("45", tid.ComponentValue, "The component value should equal the last wildcard.");
            Assert.IsNotNull(tid.For<Person>(), "Parent identities should be preserved in n-ary paths.");
            Assert.AreEqual("123", tid.For<Person>().ComponentValue, "Parent identities' component value should be preserved in n-ary paths.");
        }
        [TestMethod]
        public void PathIdProv_HappyCaseInsensitiveEquality()
        {
            var id1 = cipp.Parse("/api/person/Joost/detail/xx");
            var id2 = cipp.Parse("/API/Person/JOOST/detail/Xx");
            var id3 = cipp.Parse("/Api/Person/Jöost/detaIL/xx");
            var id4 = cipp.Parse("/Api/Person/JÖost/detaIL/xx");
            Assert.IsTrue(new[] { id1, id2, id3, id4 }.All(id => id.ForType == typeof(Detail)), "Case insensitive parsing of paths should lead to the same entity type.");
            Assert.IsTrue(id1.Equals(id2), "Case insensitive providers should ignore casing of identity component values.");
            Assert.AreNotEqual(id1.ComponentValue, id2.ComponentValue, "Different actual component values should be preserved.");
            Assert.AreNotEqual(id1.For<Person>().ComponentValue, id2.For<Person>().ComponentValue, "Different actual parent component values should be preserved.");
            Assert.AreEqual(id1, id2, "Non generic equality should adhere to the generic equality.");
            Assert.AreNotEqual(id1, id3, "Case insensitivy should treat diacritics as differences.");
            Assert.AreEqual(id3, id4, "Case insensitivity should ignore casing of characters with the same diacritics.");
        }
        [TestMethod]
        public void PathIdProv_HappyToPath()
        {
            var pid = pp.Creator<Person>().Create(123);
            var did = pp.Creator<Detail>().Create((123, 1));
            Assert.AreEqual("/api/person/123", pp.ToPath(pid), "ToPath should fill in wildcards base on component values (unary).");
            Assert.AreEqual("/api/person/123/detail/1", pp.ToPath(did), "ToPath should fill in wildcards base on component values (binary).");
        }
        [TestMethod]
        public void PathIdProv_HappyUnknown()
        {
            var up = pp.Parse("/unknown/path/123", false);
            Assert.IsNotNull(up, "PathIdentityProvider should be able to parse unknown paths.");
            Assert.AreEqual(typeof(object), up.ForType, "An unknown path is an identity for an unknown (typeof(object))");
            Assert.AreEqual("/unknown/path/123", up.Value.ToString(), "An unknown path's underlying value should be the path itself.");
        }
        [TestMethod]
        public void PathIdProv_HappyStatic()
        {
            var sid = pp.Parse("/api/static");
            Assert.IsNotNull(sid);
            Assert.AreEqual(typeof(Static), sid.ForType);
            Assert.AreEqual("", sid.Value);
            Assert.AreEqual(1, sid.Arity);
        }
        [TestMethod]
        public void PathIdProv_ArityMismatch()
        {
            Assert.ThrowsException<ArgumentException>(() => new ArityMismatchProvider(), "An exception should be raised if there are mismatches between arity and number of wildcards.");
        }

        public class ArityMismatchProvider : PathIdentityProvider
        {
            public ArityMismatchProvider()
            {
                BuildEntry(typeof(A)).WithPath("/api/a/*/*").Add();
                AddEntry("/api/a/*/*", typeof(A));
            }
        }
        public class TestPathIdentityProvider : PathIdentityProvider
        {
            public new static TestPathIdentityProvider CaseSensitive { get; } = new TestPathIdentityProvider(true);
            public new static TestPathIdentityProvider CaseInsensitive { get; } = new TestPathIdentityProvider(false);
            public TestPathIdentityProvider(bool caseSensitive) : base(caseSensitive)
            {
                AddEntry("/api/person/*", typeof(Person));
                AddEntry("/api/person/*/detail/*", typeof(Person), typeof(Detail));
                AddEntry("/api/person/*/detail/*/*", typeof(Person), typeof(Detail), typeof(Sub));
                AddEntry("/api/static", typeof(Static));
            }
        }

        public class Static { }
    }
}

