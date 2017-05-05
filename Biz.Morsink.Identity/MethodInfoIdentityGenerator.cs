using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ex = System.Linq.Expressions.Expression;
namespace Biz.Morsink.Identity
{
    public class MethodInfoIdentityGenerator<T> : IIdentityGenerator<T>
    {
        private readonly MethodInfo _method;
        private readonly IIdentityProvider _provider;
        private readonly Func<T, IIdentity<T>> _generator;

        public MethodInfoIdentityGenerator(IIdentityProvider provider, MethodInfo methodInfo)
        {
            _method = methodInfo;
            _provider = provider;
            _generator = CreateFunc();
        }

        public Type ForType => throw new NotImplementedException();

        public IIdentity<T> New(T entity)
            => _generator(entity);

        public IIdentity New(object entity)
            => _generator((T)entity);

        private Func<T, IIdentity<T>> CreateFunc()
        {
            return (Func<T,IIdentity<T>>)_method.CreateDelegate(typeof(Func<T, IIdentity<T>>), _provider);
        }
    }
}
