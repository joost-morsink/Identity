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
            Assert.AreEqual(1, id.Arity, "A single string should have arity 1.");

            var resid = idprov.Translate(id);
            Assert.AreEqual(3, resid.Arity, "A separated string of three parts should have arity 3.");

            Assert.IsTrue(idprov.Equals(resid, id), "According to an identity provider that can read separated strings, two 'equal' values of different arities should be equal.");
            Assert.IsTrue(idprov.Equals(id, resid), "Equality should be symmetric.");
        }
        [TestMethod]
        public void Arity_HappyBack()
        {
            var id = idprov.SubId(1, 2, 3);
            Assert.AreEqual(3, id.Arity, "An identity value of 3 components should have arity 3.");

            var resid = id.MakeFree().WithType<string>();
            Assert.AreEqual(typeof(Sub), resid.ForType, "Making an identity free should preserve the ForType.");
            Assert.AreEqual(1, resid.Arity, "Making an identity free should respect the arity of the desired underlying value.");
            Assert.AreEqual("1-2-3", resid.Value.First(), "A 3-ary value should be able to convert to a separated string.");
        }

        [TestMethod]
        public void Arity_Happy2ProvidersOutSep()
        {
            var id = genprov.Creator<Sub>().Create("1|2|3");
            Assert.AreEqual(1, id.Arity, "GenericIdentityProviders should create values of arity 1");

            var resid = idprov.Translate(id);
            Assert.AreEqual(3, resid.Arity, "Translated identity values should respect the arity of the importing identity provider.");
            Assert.IsInstanceOfType(resid.Value, typeof((int, int, int)), "Translated identity values should respect the underlying value type of the importing identity provider.");
            Assert.AreEqual((1, 2, 3), resid.Value, "Translated identity values should convert correctly.");

            Assert.IsTrue(idprov.Equals(resid, id), "The converted value should equal the original value.");
            Assert.IsTrue(idprov.Equals(id, resid), "Equality should be symmetric.");
        }
        [TestMethod]
        public void Arity_Happy2ProvidersInSep()
        {
            var id = genprov.Creator<Sub>().Create("1-2-3");
            var resid = idprov.Translate(id);
            Assert.AreEqual(3, resid.Arity, "Translated identity values should respect the arity of the importing identity provider.");
            Assert.IsInstanceOfType(resid.Value, typeof((int, int, int)), "Translated identity values should respect the underlying value type of the importing identity provider.");
            Assert.AreEqual((1, 2, 3), resid.Value, "Translated identity values should convert correctly.");

            Assert.IsTrue(idprov.Equals(resid, id), "The converted value should equal the original value.");
            Assert.IsTrue(idprov.Equals(id, resid), "Equality should be symmetric.");
        }

        [TestMethod]
        public void Arity_Happy2ProvidersBack()
        {
            var id = idprov.Creator<Sub>().Create("1-2-3");
            Assert.AreEqual(3, id.Arity, "Created identity values should respect the arity of the identity provider.");
            Assert.IsInstanceOfType(id.Value, typeof((int, int, int)), "Created identity values should respect the underlying value type of the importing identity provider.");
            Assert.AreEqual((1, 2, 3), id.Value, "Created identity values should convert correctly.");

            var resid = genprov.Translate(id);
            Assert.AreEqual(1, resid.Arity, "Translated identity values should respect the arity of the importing identity provider.");
            Assert.IsInstanceOfType(resid.Value, typeof(string), "Translated identity values should respect the underlying value type of the importing identity provider.");
            Assert.AreEqual("1|2|3", resid.Value, "Translated identity values should convert correctly.");

            Assert.IsTrue(genprov.Equals(resid, id), "The converted value should equal the original value.");
            Assert.IsTrue(genprov.Equals(id, resid), "Equality should be symmetric.");
        }
    }
}
