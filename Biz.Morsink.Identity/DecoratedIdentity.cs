using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity
{
    public class DecoratedIdentity<D, I, T> : IIdentity<T>, IIdentityContainer
        where I : IIdentity<T>
        where D : IIdentity
    {
        public DecoratedIdentity(D decoration, I identity)
        {
            Decoration = decoration;
            Identity = identity;
        }

        public D Decoration { get; }
        public I Identity { get; }

        public IIdentityProvider Provider => Identity.Provider;

        public Type ForType => typeof(T);

        public object Value => Identity.Value;

        public object ComponentValue => Identity.ComponentValue;

        public IEnumerable<IIdentity> Identities => Identity.Identities.Concat(Decoration.Identities);

        public int Arity => Identity.Arity;

        public IIdentity Map(IIdentity id)
            => (IIdentity)Activator.CreateInstance(typeof(DecoratedIdentity<,>).MakeGenericType(typeof(D), id.ForType), Decoration, id);
        public IIdentity<U> Map<U>(IIdentity<U> id)
            => new DecoratedIdentity<D, IIdentity<U>, U>(Decoration, id);
    }
    public class DecoratedIdentity<D, T> : DecoratedIdentity<D, IIdentity<T>, T>
        where D : IIdentity
    {
        public DecoratedIdentity(D decoration, IIdentity<T> identity) : base(decoration, identity) { }
    }
    public static class DecoratedIdentityExt
    {
        public static DecoratedIdentity<D, T> Decorate<D, T>(this IIdentity<T> id, D decoration)
            where D : IIdentity
            => new DecoratedIdentity<D, T>(decoration, id);
    }
}
