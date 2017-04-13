using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity
{
    public static class Utility
    {
        public static IdentityCreator<T> Creator<T>(this IIdentityProvider provider)
            => new IdentityCreator<T>(provider);
        public static IdentityCreator Creator(this IIdentityProvider provider, Type type)
            => new IdentityCreator(provider, type);
        public static IIdentity<T> For<T>(this IIdentity id)
            => id.Identities.OfType<IIdentity<T>>().FirstOrDefault();
    }
}
