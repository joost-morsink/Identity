using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public struct IdentityGenerator<T> : IIdentityGenerator<T>
    {
        private readonly IIdentityProvider _provider;

        public IdentityGenerator(IIdentityProvider provider)
        {
            _provider = provider;
        }

        public Type ForType => typeof(T);

        public IIdentity<T> New(T entity)
            => _provider.New(entity);

        IIdentity IIdentityGenerator.New(object entity)
            => New((T)entity);
    }
    public struct IdentityGenerator : IIdentityGenerator
    {
        private readonly IIdentityProvider _provider;
        private readonly Type _type;

        public IdentityGenerator(IIdentityProvider provider, Type type)
        {
            _provider = provider;
            _type = type;
        }

        public Type ForType => throw new NotImplementedException();

        public IIdentity New(object entity)
            => _provider.New(_type, entity);
    }
}
