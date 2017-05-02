using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Biz.Morsink.DataConvert;

namespace Biz.Morsink.Identity.Converters
{
    public class ToStringConverter : DataConvert.Converters.ToStringConverter, IConverter
    {
        public ToStringConverter() : base(false, System.Globalization.CultureInfo.InvariantCulture) { }
        bool IConverter.CanConvert(Type from, Type to)
            => CanConvert(from,to) 
            && !typeof(ILateIdentityValue).GetTypeInfo().IsAssignableFrom(from.GetTypeInfo())
            && !typeof(IIdentity).GetTypeInfo().IsAssignableFrom(from.GetTypeInfo())
    }
}
