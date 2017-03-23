using System;
using System.Collections.Generic;

namespace Biz.Morsink.Identity
{
    public interface IIdentity
    {
        IIdentityProvider Provider { get; }
        Type ForType { get; }
        object Value { get; }
        IEnumerable<IIdentity> Identities { get; }
        int Arity { get; }
    }
    public interface IIdentity<T> : IIdentity
    {
        object ComponentValue { get; }
    }
    public interface IIdentity<T, U> : IMultiaryIdentity<U, IIdentity<T>>
    {
    }
    public interface IIdentity<T, U, V> : IMultiaryIdentity<V, IIdentity<T, U>>
    {
    }
    public interface IIdentity<T, U, V, W> : IMultiaryIdentity<W, IIdentity<T, U, V>>
    {
    }
    public interface IIdentity<T, U, V, W, X> : IMultiaryIdentity<X, IIdentity<T, U, V, W>>
    {
    }
}
