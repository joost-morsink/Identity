using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Biz.Morsink.DataConvert;
using Ex = System.Linq.Expressions.Expression;
using static Biz.Morsink.Identity.Converters.Helpers;
namespace Biz.Morsink.Identity.Converters
{
    public class LateIdentityConverter : IConverter, IDataConverterRef
    {
        public IDataConverter Ref { get; set; }

        public bool CanConvert(Type from, Type to)
            => typeof(ILateIdentityValue).GetTypeInfo().IsAssignableFrom(from.GetTypeInfo());

        public Delegate Create(Type from, Type to)
        {
            var q = (from ty in @from.GetTypeInfo().ImplementedInterfaces.Concat(new[] { @from })
                     let ti = ty.GetTypeInfo()
                     where ti.IsInterface
                     let gps = ti.GenericTypeArguments
                     where gps.Length == 1 && ti.GetGenericTypeDefinition() == typeof(ILateIdentityValue<>)
                     select gps[0]).FirstOrDefault();
            if (q == null)
                return createGeneral(from, to);
            else
                return createGeneric(from, to, q);
        }
        private Delegate createGeneral(Type from, Type to)
        {
            var iliv = typeof(ILateIdentityValue).GetTypeInfo();
            var input = Ex.Parameter(from, "input");
            var block = Ex.Condition(Ex.Property(input, iliv.GetDeclaredProperty(nameof(ILateIdentityValue.IsAvailable))),
                Ex.Call(typeof(DataConverterExt), nameof(DataConverterExt.DoConversion), new[] { typeof(object), to },
                    Ex.Constant(Ref),
                    Ex.Property(input, iliv.GetDeclaredProperty(nameof(ILateIdentityValue.Value)))),
                NoResult(to));
            var lambda = Ex.Lambda(block, input);
            return lambda.Compile();
        }
        private Delegate createGeneric(Type from, Type to, Type keyType)
        {
            var iliv = typeof(ILateIdentityValue).GetTypeInfo();
            var ilivk = typeof(ILateIdentityValue<>).MakeGenericType(keyType).GetTypeInfo();
            var input = Ex.Parameter(from, "input");
            var block = to == keyType
                ? Ex.Condition(Ex.Property(input, iliv.GetDeclaredProperty(nameof(ILateIdentityValue.IsAvailable))),
                    Result(to, Ex.Property(input, ilivk.GetDeclaredProperty(nameof(ILateIdentityValue.Value)))),
                    NoResult(to))
                : Ex.Condition(Ex.Property(input, iliv.GetDeclaredProperty(nameof(ILateIdentityValue.IsAvailable))),
                    Ex.Call(typeof(DataConverterExt), nameof(DataConverterExt.DoConversion), new[] { keyType, to },
                        Ex.Constant(Ref),
                        Ex.Property(input, ilivk.GetDeclaredProperty(nameof(ILateIdentityValue.Value)))),
                    NoResult(to));
            var lambda = Ex.Lambda(block, input);
            return lambda.Compile();
        }
    }
}
