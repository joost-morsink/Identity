using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ex = System.Linq.Expressions.Expression;

namespace Biz.Morsink.Identity
{
    public interface IValueToIdentity
    {
        IIdentity Create<K>(K value);
    }
    public interface IValueToIdentity<T> : IValueToIdentity
    {
        new IIdentity<T> Create<K>(K value);
    }
    public class ValueToIdentity<T> : IValueToIdentity<T>
    {
        private readonly MethodInfo _mi;
        private readonly ConcurrentDictionary<Type, Delegate> _delegates;
        private readonly IIdentityProvider _provider;

        public ValueToIdentity(IIdentityProvider provider, MethodInfo mi)
        {
            _provider = provider;
            _mi = mi;
            _delegates = new ConcurrentDictionary<Type, Delegate>();
        }
        private Func<K, IIdentity<T>> getDelegate<K>()
        {
            return _delegates.GetOrAdd(typeof(K), type =>
            {
                var input = Ex.Parameter(type, "input");
                var block = Ex.Call(Ex.Constant(_provider), _mi.MakeGenericMethod(type), input);
                var lambda = Ex.Lambda(block, input);
                return lambda.Compile();
            }) as Func<K, IIdentity<T>>;
        }
        public IIdentity<T> Create<K>(K value)
        {
            var del = getDelegate<K>();
            return del(value);
        }
        IIdentity IValueToIdentity.Create<K>(K value) => Create(value);
    }
}
