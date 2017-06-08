using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Biz.Morsink.DataConvert;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class GenericIdentityProviderTest
    {
        private GenericIdentityProvider<string> provider;

        [TestInitialize]
        public void Init()
        {
            provider = new GenericIdentityProvider<string>(DataConverter.Default);
        }
        [TestMethod]
        public void GenericIp_Happy()
        {
            Assert.AreEqual("42", provider.Creator<A>().Create(42).Value, "GenericIdentityProvider should convert to the correct underlying type.");
            Assert.AreEqual("xx", provider.Creator<A>().Create("xx").ComponentValue,"GenericIdentityProvider preserves the underlying value during creation.");
        }
        [TestMethod]
        public void GenericIp_HappyTranslate()
        {
            Assert.AreEqual("42", provider.Translate(TestIdProvider.Instance.Creator<A>().Create(42)).Value, "A translated identity value should respect the type constraint of the GenericIdentityProvider.");
            Assert.AreNotEqual(provider, TestIdProvider.Instance.Creator<A>().Create(42).Provider, "A non translated identity value should not refer to the GenericIdentityProvider.");
            Assert.AreEqual(provider, provider.Translate(TestIdProvider.Instance.Creator<A>().Create(42)).Provider, "A translated identity value should refer to the GenericIdentityProvider.");
        }
        [TestMethod]
        public void GenericIp_HappyEquality()
        {
            Assert.IsTrue(provider.Equals(provider.Creator<A>().Create("42"), TestIdProvider.Instance.AId(42)), "Two identity values should be considered equal if their converted underlying values are equal.");
        }
    }
}
