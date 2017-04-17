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
            Assert.AreEqual("42", provider.Creator<A>().Create(42).Value);
            Assert.AreEqual("xx", provider.Creator<A>().Create("xx").ComponentValue);
        }
        [TestMethod]
        public void GenericIp_HappyTranslate()
        {
            Assert.AreEqual("42", provider.Translate(IdProvider.Instance.Creator<A>().Create(42)).Value);
            Assert.AreNotEqual(provider, IdProvider.Instance.Creator<A>().Create(42).Provider);
            Assert.AreEqual(provider, provider.Translate(IdProvider.Instance.Creator<A>().Create(42)).Provider);
        }
        [TestMethod]
        public void GenericIp_HappyEquality()
        {
            Assert.IsTrue(provider.Equals(provider.Creator<A>().Create("42"), IdProvider.Instance.AId(42)));
        }
    }
}
