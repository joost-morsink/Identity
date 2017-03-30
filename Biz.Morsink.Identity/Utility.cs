using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public static class Utility
    {
        public static IdBuilder<T> Builder<T>(this IIdentityProvider provider)
            => new IdBuilder<T>(provider);
        public struct IdBuilder<T>
        {
            private readonly IIdentityProvider _prov;

            internal IdBuilder(IIdentityProvider prov)
            {
                _prov = prov;
            }
            public Identity<T, K> Create<K>(K value)
                => new Identity<T, K>(_prov, value);
        }
    }
}
