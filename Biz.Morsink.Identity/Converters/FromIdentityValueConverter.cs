using System;
using System.Linq.Expressions;
using System.Reflection;
using Biz.Morsink.DataConvert;
using Ex = System.Linq.Expressions.Expression;
namespace Biz.Morsink.Identity.Converters
{ 
    /// <summary>
    /// A converter that uses an identity value's underlying value for conversion.
    /// First it tries conversion using the identity value's provider's dataconverter.
    /// If that fails, it feeds the object typed value back into the pipeline.
    /// </summary>
    public class FromIdentityValueConverter : IConverter, IDataConverterRef
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public FromIdentityValueConverter()
        {
        }

        public IDataConverter Ref { get; set; }

        public bool SupportsLambda => true;

        public bool CanConvert(Type from, Type to)
            => typeof(IIdentity).GetTypeInfo().IsAssignableFrom(from.GetTypeInfo()); 

        public LambdaExpression CreateLambda(Type from, Type to)
        {
            var input = Ex.Parameter(from, "type");
            
            var res = Ex.Parameter(typeof(ConversionResult<>).MakeGenericType(to), "res");
            var prop = from.GetTypeInfo().GetDeclaredProperty(nameof(IIdentity.Value))
                           ?? typeof(IIdentity).GetTypeInfo().GetDeclaredProperty(nameof(IIdentity.Value));
            var block = Ex.Block(new[] { res },
                Ex.Assign(res,
                    Ex.Call(
                        typeof(DataConverterExt),
                        nameof(DataConverterExt.DoConversion),
                        new[] { typeof(object), to },
                        Ex.Call(
                            Ex.Property(input, typeof(IIdentity).GetTypeInfo().GetDeclaredProperty(nameof(IIdentity.Provider))),
                            typeof(IIdentityProvider).GetTypeInfo().GetDeclaredMethod(nameof(IIdentityProvider.GetConverter)),
                            Ex.Property(input, typeof(IIdentity).GetTypeInfo().GetDeclaredProperty(nameof(IIdentity.ForType))),
                            Ex.Constant(false)),
                        Ex.Property(input, typeof(IIdentity).GetTypeInfo().GetDeclaredProperty(nameof(IIdentity.Value))))),
                Ex.Condition(Ex.Property(res, nameof(IConversionResult.IsSuccessful)),
                    res,
                    Ex.Call(typeof(DataConverterExt), nameof(DataConverterExt.DoConversion), new[] { prop.PropertyType, to },
                        Ex.Constant(Ref), Ex.Property(input, prop))));

            var lambda = Ex.Lambda(block, input);
            return lambda;
        }

        public Delegate Create(Type from, Type to)
            => CreateLambda(from, to).Compile();
    }
}
