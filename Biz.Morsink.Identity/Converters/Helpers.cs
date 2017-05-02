using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Biz.Morsink.DataConvert;
using Ex = System.Linq.Expressions.Expression;
namespace Biz.Morsink.Identity.Converters
{
    static class Helpers
    {
        public static Ex NoResult(Type t)
            => Ex.Default(typeof(ConversionResult<>).MakeGenericType(t));
        public static Ex Result(Type t, Ex expr)
            => Ex.New(typeof(ConversionResult<>).MakeGenericType(t).GetTypeInfo().DeclaredConstructors
                .Single(ci => ci.GetParameters().Length == 1 && ci.GetParameters()[0].ParameterType == t), expr);
    }
}
