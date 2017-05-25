using System;
using Biz.Morsink.DataConvert;
using Biz.Morsink.DataConvert.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Biz.Morsink.Identity.Converters;

namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class IdentityValueConverterTest
    {
        private DataConverter converter;

        [TestInitialize]
        public void Init()
        {
            converter = new DataConverter(
                IdentityConverter.Instance,
	            new FromIdentityValueConverter(), 
	            new ToIdentityValueConverter(TestIdProvider.Instance), 
	            new ToStringConverter(false),
	            new DynamicConverter());
        }
        [TestMethod]
        public void FromIdConv_Happy()
        {
            var id = TestIdProvider.Instance.AId(42);
            Assert.AreEqual(42, converter.Convert(id).To<int>());
            Assert.AreEqual("42", converter.Convert(id).To<string>());
        }
        [TestMethod]
        public void ToIdConv_Happy(){
            var success = converter.TryConvert("42", out IIdentity<A> id);
            Assert.IsTrue(success);
            Assert.AreEqual(42, id.Value);
        }
    }
}
