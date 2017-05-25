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
            Assert.AreEqual(4, p.Count);
            Assert.AreEqual("", p[0]);
            Assert.AreEqual("api", p[1]);
            Assert.AreEqual("person", p[2]);
            Assert.AreEqual("1", p[3]);
        }
        [TestMethod]
        public void Path_Star()
        {
            var p = Path.Parse("/api/person/*/test", null);
            var q = Path.Parse("/api/person/123/test", null);
            var m = p.Match(q);
            Assert.IsNotNull(m);
            Assert.AreEqual(1, m.Parts.Count);
            Assert.AreEqual("123", m.Parts[0]);
        }
        [TestMethod]
        public void Path_DoubleStar()
        {
            var p = Path.Parse("/api/person/*/detail/*", null);
            var q = Path.Parse("/api/person/123/detail/456", null);
            var m = p.Match(q);
            Assert.IsNotNull(m);
            Assert.AreEqual(2, m.Parts.Count);
            Assert.AreEqual("123", m.Parts[0]);
            Assert.AreEqual("456", m.Parts[1]);
        }
        [TestMethod]
        public void PathTree_Happy()
        {
            var ps = new[]
            {
                Path.Parse("/api/person/*", "PERSON"),
                Path.Parse("/api/person/*/detail/*", "DETAIL"),
                Path.Parse("/api/person/*/detail/*/*", "SUB"),
                Path.Parse("/api/a/*","A")
            };
            var tree = new PathMatchTree(ps);
            var m = tree.Walk(Path.Parse("/api/person/13/detail/42", null));
            Assert.IsTrue(m.IsSuccess);
            Assert.AreEqual("DETAIL", m.Path.Data);
            Assert.AreEqual(2, m.Parts.Count);
            Assert.AreEqual("13", m.Parts[0]);
            Assert.AreEqual("42", m.Parts[1]);
        }
    }
}
