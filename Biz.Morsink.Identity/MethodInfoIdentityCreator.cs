using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Biz.Morsink.DataConvert;
using Ex = System.Linq.Expressions.Expression;
namespace Biz.Morsink.Identity
{
    /// <summary>
    /// This helper class is used by the AbstractIdentityProvider to compile lambda's for an identity value creating method.
    /// </summary>
    /// <typeparam name="T">The type of object the identity value refers to.</typeparam>
    public class MethodInfoIdentityCreator<T> : IIdentityCreator<T>
    {
        private readonly MethodInfo _method;
        private readonly ConcurrentDictionary<Type, Delegate> _delegates;
        private readonly IIdentityProvider _provider;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">The identity provider the method is called on.</param>
        /// <param name="method">The method to call.</param>
        public MethodInfoIdentityCreator(IIdentityProvider provider, MethodInfo method)
        {
            _provider = provider;
            _method = method;
            _delegates = new ConcurrentDictionary<Type, Delegate>();
        }
        /// <summary>
        /// Gets the delegate for creating an identity value.
        /// </summary>
        /// <typeparam name="K">The underlying input type for the identity value.</typeparam>
        /// <returns>A function that converts a value of type K to an identity value for objects of type T.</returns>
        private Func<K, IIdentity<T>> getDelegate<K>()
        {
            return (Func<K, IIdentity<T>>)_delegates.GetOrAdd(typeof(K), type =>
            {
                var input = Ex.Parameter(type, "input");
                var provider = Ex.Constant(_provider);
                var converter = Ex.Constant(_provider.GetConverter(typeof(T), true), typeof(IDataConverter));
                if (_method.ContainsGenericParameters)
                    return fromGenericMethod(type, input, provider);
                else if (_method.GetParameters().Length == 1)
                    return fromSingleParameterMethod<K>(input, provider, converter);
                else
                    return fromMultiParameterMethod<K>(input, provider, converter);
            });
        }
        /// <summary>
        /// This method constructs a lambda for a method returning a multiary identity value based on multiple input parameters.
        /// It compiles and returns the constructed lambda.
        /// </summary>
        private Delegate fromMultiParameterMethod<K>(ParameterExpression input, Ex provider, Ex converter)
        {
            var methodParams = _method.GetParameters();
            var tupleType = typeof(ValueTuple).GetTypeInfo().Assembly.ExportedTypes
                            .Where(t => t.Name.StartsWith("ValueTuple") && t.GetTypeInfo().GenericTypeParameters.Length == methodParams.Length)
                            .First()
                            .MakeGenericType(methodParams.Select(p => p.ParameterType).ToArray());
            var convresult = Ex.Parameter(typeof(ConversionResult<>).MakeGenericType(tupleType));
            var block = Ex.Block(new[] { convresult },
                Ex.Assign(convresult,
                    Ex.Call(typeof(DataConverterExt).GetTypeInfo().GetDeclaredMethod(nameof(DataConverterExt.DoConversion))
                            .MakeGenericMethod(typeof(K), tupleType),
                            converter, input)),
                Ex.Condition(Ex.Property(convresult, nameof(IConversionResult.IsSuccessful)),
                    Ex.Call(provider, _method, tupleType.GenericTypeArguments.Select((t, i) => Ex.Field(Ex.Property(convresult, nameof(IConversionResult.Result)), $"Item{i + 1}"))),
                    Ex.Default(_method.ReturnType)));
            var lambda = Ex.Lambda(block, input);
            return lambda.Compile();
        }
        /// <summary>
        /// This method contructs a lambda for a method returning a unary identity value based on a single typed input parameter.
        /// It compiles and returns the constructed lambda.
        /// </summary>
        private Delegate fromSingleParameterMethod<K>(ParameterExpression input, Ex provider, Ex converter)
        {
            var methodParams = _method.GetParameters();
            var convresult = Ex.Parameter(typeof(ConversionResult<>).MakeGenericType(methodParams[0].ParameterType));
            var block = Ex.Block(new[] { convresult },
                Ex.Assign(convresult,
                    Ex.Call(typeof(DataConverterExt).GetTypeInfo().GetDeclaredMethod(nameof(DataConverterExt.DoConversion))
                            .MakeGenericMethod(typeof(K), methodParams[0].ParameterType),
                            converter, input)),
                Ex.Condition(Ex.Property(convresult, nameof(IConversionResult.IsSuccessful)),
                    Ex.Call(provider, _method, Ex.Property(convresult, nameof(IConversionResult.Result))),
                    Ex.Default(_method.ReturnType)));
            var lambda = Ex.Lambda(block, input);
            return lambda.Compile();
        }
        /// <summary>
        /// This method constructs a lambda for a method returning an IIdentity&lt;T&gt; using a generically typed input parameter.
        /// </summary>
        private Delegate fromGenericMethod(Type type, ParameterExpression input, Ex provider)
        {
            var block = Ex.Call(provider, _method.MakeGenericMethod(type), input);
            var lambda = Ex.Lambda(block, input);
            return lambda.Compile();
        }
        /// <summary>
        /// Create an identity value for a type T.
        /// </summary>
        /// <typeparam name="K">The underlying type of the identity value.</typeparam>
        /// <param name="value">The underlying value of the identity value.</param>
        /// <returns>A newly constructed identity value for type T with the specified underlying value.</returns>
        public IIdentity<T> Create<K>(K value)
        {
            var del = getDelegate<K>();
            return del(value);
        }
        IIdentity IIdentityCreator.Create<K>(K value) => Create(value);
    }
}
