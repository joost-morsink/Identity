using System;
using System.Linq;
using System.Reflection;
using Biz.Morsink.DataConvert;
using Ex = System.Linq.Expressions.Expression;
using Et = System.Linq.Expressions.ExpressionType;

namespace Biz.Morsink.Identity.Converters
{
    /// <summary>
    /// This component converts values to identity values. 
    /// It is basically the IConverter interface implementation for IIdentityProvider.
    /// </summary>
    public class ToIdentityValueConverter : IConverter
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider"></param>
        public ToIdentityValueConverter(IIdentityProvider provider)
        {
            Provider = provider;
        }
        /// <summary>
        /// The identity provider that is used to create the identity values.
        /// </summary>
        public IIdentityProvider Provider { get; }

        private Type selectIdentityInterface(Type type)
        {
            if (type.GetTypeInfo().GenericTypeArguments.Length == 1
                    && type.GetTypeInfo().GetGenericTypeDefinition() == typeof(IIdentity<>))
                return type.GetTypeInfo().GenericTypeArguments[0];
            else
                return null;
        }
        public bool CanConvert(Type from, Type to)
        {
            var t = selectIdentityInterface(to);
            return t != null && Provider.Supports(t);
        }

        public Delegate Create(Type from, Type to)
        {
            var input = Ex.Parameter(from, "input");
            var entType = selectIdentityInterface(to);
            var res = Ex.Parameter(to, "res");
            var block = Ex.Block(new[] { res },
                Ex.Assign(res,
                    Ex.Call(Ex.Constant(Provider),
                        typeof(IIdentityProvider).GetTypeInfo()
                            .GetDeclaredMethods(nameof(IIdentityProvider.Create))
                            .Single(mi => mi.GetGenericArguments().Length == 2)
                            .MakeGenericMethod(entType, from),
                        input)),
                Ex.Condition(Ex.MakeBinary(Et.Equal, res, Ex.Default(to)),
                    NoResult(to),
                    Result(to, res)));
            var lambda = Ex.Lambda(block, input);
            return lambda.Compile();
        }
        static Ex NoResult(Type t)
            => Ex.Default(typeof(ConversionResult<>).MakeGenericType(t));
        static Ex Result(Type t, Ex expr)
            => Ex.New(typeof(ConversionResult<>).MakeGenericType(t).GetTypeInfo().DeclaredConstructors
                .Single(ci => ci.GetParameters().Length == 1 && ci.GetParameters()[0].ParameterType == t), expr);
    }
}
