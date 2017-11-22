using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.Utils
{
    public static class Manipulations
    {
        #region Helper morph structs
        public struct IdentityConverter<T>
        {
            private readonly IIdentity<T> id;

            public IdentityConverter(IIdentity<T> id)
            {
                this.id = id;
            }
            public IIdentity<R> To<R>()
                => id.Provider.Creator<R>().Create(id.Value);
            public bool TryTo<R>(out IIdentity<R> identity)
            {
                identity = To<R>();
                return identity != null;
            }
        }
        public struct IdentityConverter<T, U>
        {
            private readonly IIdentity<T, U> id;

            public IdentityConverter(IIdentity<T, U> id)
            {
                this.id = id;
            }
            public IIdentity<T, R> To<R>()
                => id.Provider.Creator<R>().Create(id.Value) as IIdentity<T, R>;
            public bool TryTo<R>(out IIdentity<T, R> identity)
            {
                identity = To<R>();
                return identity != null;
            }
        }
        public struct IdentityConverter<T, U, V>
        {
            private readonly IIdentity<T, U, V> id;

            public IdentityConverter(IIdentity<T, U, V> id)
            {
                this.id = id;
            }
            public IIdentity<T, U, R> To<R>()
                => id.Provider.Creator<R>().Create(id.Value) as IIdentity<T, U, R>;
            public bool TryTo<R>(out IIdentity<T, U, R> identity)
            {
                identity = To<R>();
                return identity != null;
            }
        }
        public struct IdentityConverter<T, U, V, W>
        {
            private readonly IIdentity<T, U, V, W> id;

            public IdentityConverter(IIdentity<T, U, V, W> id)
            {
                this.id = id;
            }
            public IIdentity<T, U, V, R> To<R>()
                => id.Provider.Creator<R>().Create(id.Value) as IIdentity<T, U, V, R>;
            public bool TryTo<R>(out IIdentity<T, U, V, R> identity)
            {
                identity = To<R>();
                return identity != null;
            }
        }
        public struct IdentityConverter<T, U, V, W, X>
        {
            private readonly IIdentity<T, U, V, W, X> id;

            public IdentityConverter(IIdentity<T, U, V, W, X> id)
            {
                this.id = id;
            }
            public IIdentity<T, U, V, W, R> To<R>()
                => id.Provider.Creator<R>().Create(id.Value) as IIdentity<T, U, V, W, R>;
            public bool TryTo<R>(out IIdentity<T, U, V, W, R> identity)
            {
                identity = To<R>();
                return identity != null;
            }
        }
        #endregion

        public static IdentityConverter<T> Morph<T>(this IIdentity<T> baseId)
            => new IdentityConverter<T>(baseId);
        public static IdentityConverter<T, U> Morph<T, U>(this IIdentity<T, U> baseId)
            => new IdentityConverter<T, U>(baseId);
        public static IdentityConverter<T, U, V> Morph<T, U, V>(this IIdentity<T, U, V> baseId)
            => new IdentityConverter<T, U, V>(baseId);
        public static IdentityConverter<T, U, V, W> Morph<T, U, V, W>(this IIdentity<T, U, V, W> baseId)
            => new IdentityConverter<T, U, V, W>(baseId);
        public static IdentityConverter<T, U, V, W, X> Morph<T, U, V, W, X>(this IIdentity<T, U, V, W, X> baseId)
             => new IdentityConverter<T, U, V, W, X>(baseId);
    }
}
