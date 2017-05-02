using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Biz.Morsink.DataConvert;
using Biz.Morsink.DataConvert.Converters;
using Biz.Morsink.Identity.Converters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Biz.Morsink.Identity.Test
{
    [TestClass]
    public class LateIdentityTest
    {
        [TestMethod]
        public void LateId_Happy()
        {
            var lid = new LateIdentityValue<int>();
            Assert.IsFalse(lid.IsAvailable);
            lid.Resolve(42);
            Assert.IsTrue(lid.IsAvailable);
            Assert.AreEqual(42, lid.Value);
        }
        [TestMethod]
        public void LateIdConv_Happy()
        {
            var conv = new DataConverter(IdentityConverter.Instance, 
                new ToStringConverter(false).Restrict((from,to) => !typeof(ILateIdentityValue).GetTypeInfo().IsAssignableFrom(from)), 
                new LateIdentityConverter());
            var c = conv.GetConverter<LateIdentityValue<int>, int>();
            var lid = new LateIdentityValue<int>();
            Assert.IsFalse(c(lid).IsSuccessful);
            lid.Resolve(42);
            Assert.IsTrue(c(lid).IsSuccessful);
            Assert.AreEqual(42, c(lid).Result);

            var cs = conv.GetConverter<LateIdentityValue<int>, string>();
            lid = new LateIdentityValue<int>();
            Assert.IsFalse(cs(lid).IsSuccessful);
            lid.Resolve(42);
            Assert.IsTrue(cs(lid).IsSuccessful);
            Assert.AreEqual("42", cs(lid).Result);
        }
    }
}
