using Biz.Morsink.DataConvert;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class ArityConversionTest 
    {
        private TestIdProvider idprov;
        private GenericIdentityProvider<string> genprov;

        [TestInitialize]
        public void Init()
        {
            idprov = new TestIdProvider();
            genprov = new GenericIdentityProvider<string>(AbstractIdentityProvider.Converters.WithSeparator('|'));
        }
        [TestMethod]
        public void Arity_Happy()
        {
            var id = FreeIdentity<Sub>.Create("1-2-3");
            Assert.AreEqual(1, id.Arity);

            var resid = idprov.Translate(id);
            Assert.AreEqual(3, resid.Arity);

            Assert.IsTrue(idprov.Equals(resid, id));
            Assert.IsTrue(idprov.Equals(id, resid));
        }
        [TestMethod]
        public void Arity_HappyBack()
        {
            var id = idprov.SubId(1, 2, 3);
            Assert.AreEqual(3, id.Arity);

            var resid = id.MakeFree().WithType<string>();
            Assert.AreEqual(typeof(Sub), resid.ForType);
            Assert.AreEqual(1, resid.Arity);
            Assert.AreEqual("1-2-3", resid.Value.First());
        }

        [TestMethod]
        public void Arity_Happy2ProvidersOutSep()
        {
            var id = genprov.Creator<Sub>().Create("1|2|3");
            Assert.AreEqual(1, id.Arity);

            var resid = idprov.Translate(id);
            Assert.AreEqual(3, resid.Arity);
            Assert.AreEqual((1, 2, 3), resid.Value);

            Assert.IsTrue(idprov.Equals(resid, id));
            Assert.IsTrue(idprov.Equals(id, resid));
        }
        [TestMethod]
        public void Arity_Happy2ProvidersInSep()
        {
            var id = genprov.Creator<Sub>().Create("1-2-3");
            Assert.AreEqual(1, id.Arity);

            var resid = idprov.Translate(id);
            Assert.AreEqual(3, resid.Arity);
            Assert.AreEqual((1, 2, 3), resid.Value);

            Assert.IsTrue(idprov.Equals(resid, id));
            Assert.IsTrue(idprov.Equals(id, resid));
        }

        [TestMethod]
        public void Arity_Happy2ProvidersBack()
        {
            var id = idprov.Creator<Sub>().Create("1-2-3");
            Assert.AreEqual(3, id.Arity);
            Assert.AreEqual((1, 2, 3), id.Value);

            var resid = genprov.Translate(id);
            Assert.AreEqual(1, resid.Arity);
            Assert.AreEqual("1|2|3", resid.Value);

            Assert.IsTrue(genprov.Equals(resid, id));
            Assert.IsTrue(genprov.Equals(id, resid));
        }
    }
}
