using System;
using System.Collections.Generic;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// Base interface for all identity values.
    /// </summary>
    public interface IIdentity
    {
        /// <summary>
        /// Gets the identity value's identity provider.
        /// </summary>
        IIdentityProvider Provider { get; }
        /// <summary>
        /// Gets what type the identity value refers to.
        /// </summary>
        Type ForType { get; }
        /// <summary>
        /// Gets the identity value's actual value.
        /// </summary>
        object Value { get; }
        /// <summary>
        /// Gets the component value corresponding to the T type.
        /// </summary>
        object ComponentValue { get; }
        /// <summary>
        /// Gets all the identity values contained in this identity value (including a selfreference).
        /// </summary>
        IEnumerable<IIdentity> Identities { get; }
        /// <summary>
        /// Gets the arity of the identity value.
        /// </summary>
        int Arity { get; }
    }
    /// <summary>
    /// This interface describes identity values that refer to a certain type.
    /// </summary>
    /// <typeparam name="T">The type of object an instance is an identity value for.</typeparam>
    public interface IIdentity<T> : IIdentity
    {
    }
    /// <summary>
    /// This interface describes identity values of arity 2.
    /// </summary>
    /// <typeparam name="T">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="U">The type of object an instance is primarily an identity value for.</typeparam>
    public interface IIdentity<T, U> : IMultiaryIdentity<U, IIdentity<T>>
    {
    }
    /// <summary>
    /// This interface describes identity values of arity 3.
    /// </summary>
    /// <typeparam name="T">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="U">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="V">The type of object an instance is primarily an identity value for.</typeparam>
    public interface IIdentity<T, U, V> : IMultiaryIdentity<V, IIdentity<T, U>>
    {
    }
    /// <summary>
    /// This interface describes identity values of arity 4.
    /// </summary>
    /// <typeparam name="T">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="U">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="V">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="W">The type of object an instance is primarily an identity value for.</typeparam>
    public interface IIdentity<T, U, V, W> : IMultiaryIdentity<W, IIdentity<T, U, V>>
    {
    }
    /// <summary>
    /// This interface describes identity values of arity 5.
    /// </summary>
    /// <typeparam name="T">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="U">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="V">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="W">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="X">The type of object an instance is primarily an identity value for.</typeparam>
    public interface IIdentity<T, U, V, W, X> : IMultiaryIdentity<X, IIdentity<T, U, V, W>>
    {
    }
}
