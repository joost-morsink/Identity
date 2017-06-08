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
            Assert.AreEqual(42, converter.Convert(id).To<int>(), "An identity value should be convertible to its underlying type.");
            Assert.AreEqual("42", converter.Convert(id).To<string>(), "An identity value should be convertible to string.");
        }
        [TestMethod]
        public void ToIdConv_Happy()
        {
            var success = converter.TryConvert("42", out IIdentity<A> id);
            Assert.IsTrue(success, "A convertible underlying value should be convertible to an identity value if the type is known.");
            Assert.AreEqual(42, id.Value, "Conversion of an underlying value through an intermediate value to an identity value should preserve the semantic value of the original value.");
        }
    }
}
