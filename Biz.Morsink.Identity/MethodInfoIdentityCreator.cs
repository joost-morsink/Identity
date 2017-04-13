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
    public class MethodInfoIdentityCreator<T> : IIdentityCreator<T>
    {
        private readonly MethodInfo _mi;
        private readonly ConcurrentDictionary<Type, Delegate> _delegates;
        private readonly IIdentityProvider _provider;

        public MethodInfoIdentityCreator(IIdentityProvider provider, MethodInfo mi)
        {
            _provider = provider;
            _mi = mi;
            _delegates = new ConcurrentDictionary<Type, Delegate>();
        }
        private Func<K, IIdentity<T>> getDelegate<K>()
        {
            return (Func<K, IIdentity<T>>)_delegates.GetOrAdd(typeof(K), type =>
            {
                var input = Ex.Parameter(type, "input");
                var provider = Ex.Constant(_provider);
                var converter = Ex.Constant(_provider.GetConverter(typeof(T), true), typeof(IDataConverter));
                if (_mi.ContainsGenericParameters)
                    return fromGenericMethod(type, input, provider);
                else if (_mi.GetParameters().Length == 1)
                    return fromSingleParameterMethod<K>(input, provider, converter);
                else
                    return fromMultiParameterMethod<K>(input, provider, converter);
            });
        }

        private Delegate fromMultiParameterMethod<K>(ParameterExpression input, Ex provider, Ex converter)
        {
            var methodParams = _mi.GetParameters();
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
                    Ex.Call(provider, _mi, tupleType.GenericTypeArguments.Select((t, i) => Ex.Field(Ex.Property(convresult, nameof(IConversionResult.Result)), $"Item{i + 1}"))),
                    Ex.Default(_mi.ReturnType)));
            var lambda = Ex.Lambda(block, input);
            return lambda.Compile();
        }

        private Delegate fromSingleParameterMethod<K>(ParameterExpression input, Ex provider, Ex converter)
        {
            var methodParams = _mi.GetParameters();
            var convresult = Ex.Parameter(typeof(ConversionResult<>).MakeGenericType(methodParams[0].ParameterType));
            var block = Ex.Block(new[] { convresult },
                Ex.Assign(convresult,
                    Ex.Call(typeof(DataConverterExt).GetTypeInfo().GetDeclaredMethod(nameof(DataConverterExt.DoConversion))
                            .MakeGenericMethod(typeof(K), methodParams[0].ParameterType),
                            converter, input)),
                Ex.Condition(Ex.Property(convresult, nameof(IConversionResult.IsSuccessful)),
                    Ex.Call(provider, _mi, Ex.Property(convresult, nameof(IConversionResult.Result))),
                    Ex.Default(_mi.ReturnType)));
            var lambda = Ex.Lambda(block, input);
            return lambda.Compile();
        }

        private Delegate fromGenericMethod(Type type, ParameterExpression input, Ex provider)
        {
            var block = Ex.Call(provider, _mi.MakeGenericMethod(type), input);
            var lambda = Ex.Lambda(block, input);
            return lambda.Compile();
        }

        public IIdentity<T> Create<K>(K value)
        {
            var del = getDelegate<K>();
            return del(value);
        }
        IIdentity IIdentityCreator.Create<K>(K value) => Create(value);
    }
}
