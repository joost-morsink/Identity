using System;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// This interface exposes a method to create identity values for a certain unknown type.
    /// The type is only unknown at compile-time, because it will have a definite value at run-time.
    /// </summary>
    public interface IIdentityCreator
    {
        /// <summary>
        /// Create an identity value for the unknown type.
        /// </summary>
        /// <typeparam name="K">The underlying type of the identity value.</typeparam>
        /// <param name="value">The underlying value of the identity value.</param>
        /// <returns>A newly constructed identity value for the unknown type with the specified underlying value.</returns>
        IIdentity Create<K>(K value);
    }
    /// <summary>
    /// This interface exposes a method to create identity values for a certain type.
    /// </summary>
    /// <typeparam name="T">The type of object the created identity values will refer to.</typeparam>
    public interface IIdentityCreator<T> : IIdentityCreator
    {
        /// <summary>
        /// Create an identity value for a type T.
        /// </summary>
        /// <typeparam name="K">The underlying type of the identity value.</typeparam>
        /// <param name="value">The underlying value of the identity value.</param>
        /// <returns>A newly constructed identity value for type T with the specified underlying value.</returns>
        new IIdentity<T> Create<K>(K value);
    }
}
