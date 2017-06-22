using Biz.Morsink.Identity.PathProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class PathTest
    {
        [TestMethod]
        public void Path_Trivial()
        {
            var p = Path.Parse("/api/person/1", null);
            Assert.AreEqual(4, p.Count, "Count property should count all parts.");
            Assert.IsTrue(p[0] == "" && p[1] == "api" && p[2] == "person" && p[3] == "1", "The parsed path should match the parts in number and order of the parts in the original path string.");
        }
        [TestMethod]
        public void Path_NoStar()
        {
            var p = Path.Parse("/api/home");
            var q = Path.Parse("/api/home");
            var m = p.Match(q);
            Assert.IsTrue(m.IsSuccessful, "Two equal paths should match.");
            Assert.AreEqual(0, m.Parts.Count, "No wildcards should result in a 0-ary match.");
        }
        [TestMethod]
        public void Path_Star()
        {
            var p = Path.Parse("/api/person/*/test", null);
            var q = Path.Parse("/api/person/123/test", null);
            var m = p.Match(q);
            Assert.IsTrue(m.IsSuccessful, "A wildcard should match any value.");
            Assert.AreEqual(1, m.Parts.Count, "One wildcard should result in a unary match.");
            Assert.AreEqual("123", m.Parts[0], "The matched wildcard part should match the one in the path string.");
        }
        [TestMethod]
        public void Path_DoubleStar()
        {
            var p = Path.Parse("/api/person/*/detail/*", null);
            var q = Path.Parse("/api/person/123/detail/456", null);
            var m = p.Match(q);
            Assert.IsTrue(m.IsSuccessful, "Two wildcards should each match any value.");
            Assert.AreEqual(2, m.Parts.Count, "Two wildcards should result in a binary match.");
            Assert.IsTrue(m.Parts[0] == "123" && m.Parts[1] == "456", "The matched wildcard parts should match those in the path string in the same order.");
        }
        [TestMethod]
        public void PathTree_Happy()
        {
            var ps = new[]
            {
                Path.Parse("/api/person/*", typeof(Person)),
                Path.Parse("/api/person/*/detail/*", typeof(Detail)),
                Path.Parse("/api/person/*/detail/*/*", typeof(Sub)),
                Path.Parse("/api/a/*",typeof(A))
            };
            var tree = new PathMatchTree(ps);
            var m = tree.Walk(Path.Parse("/api/person/13/detail/42", null));
            Assert.IsTrue(m.IsSuccessful, "Walking a matchtree path on a 'known' path should succeed.");
            Assert.AreEqual(typeof(Detail), m.Path.ForType, "Walking a matchtree path on a 'known' path should find the correct path.");
            Assert.AreEqual(2, m.Parts.Count, "Two wildcards should result in a binary match.");
            Assert.IsTrue(m.Parts[0] == "13" && m.Parts[1] == "42", "The matched wildcard parts should match those in the path string in the same order.");
        }
    }
}
