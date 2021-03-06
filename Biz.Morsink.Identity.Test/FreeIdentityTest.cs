﻿using System;
using System.Collections.Generic;
using System.Text;
using Biz.Morsink.DataConvert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class FreeIdentityTest
    {
        private TestIdProvider idprov;

        [TestInitialize]
        public void Init()
        {
            idprov = new TestIdProvider();
        }

        [TestMethod]
        public void FreeId_Happy()
        {
            var id = FreeIdentity<Detail>.Create(FreeIdentity<Person>.Create(42), 1);
            var tid = idprov.Translate(id);
            Assert.IsNotNull(tid, "A compatible FreeIdentity should be importable.");
            Assert.AreEqual(1, tid.ComponentValue, "Translation of FreeIdentities should preserve the ComponentValue property.");
            Assert.AreEqual(42, ((IMultiaryIdentity)tid).Parent.ComponentValue, "Translation of FreeIdentities should preserve the ComponentValues in the Parent chain.");
        }
    }
}
