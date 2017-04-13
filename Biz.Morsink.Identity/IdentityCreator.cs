using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public struct IdentityCreator<T> : IIdentityCreator<T>
    {
        private readonly IIdentityProvider _provider;

        public IdentityCreator(IIdentityProvider provider)
        {
            _provider = provider;
        }

        public IIdentity<T> Create<K>(K value)
            => _provider.Create<T, K>(value);

        IIdentity IIdentityCreator.Create<K>(K value)
            => Create(value);
    }
    public struct IdentityCreator : IIdentityCreator
    {
        private readonly IIdentityProvider _provider;
        private readonly Type _type;
        public IdentityCreator(IIdentityProvider provider, Type type)
        {
            _provider = provider;
            _type = type;
        }

        public IIdentity Create<K>(K value)
            => _provider.Create(_type, value);
    }
}
