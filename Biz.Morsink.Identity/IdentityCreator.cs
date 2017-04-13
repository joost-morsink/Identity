using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// A utility identity creator that takes an IIdentityProvider reference.
    /// </summary>
    /// <typeparam name="T">The type of object the created identity values refer to.</typeparam>
    public struct IdentityCreator<T> : IIdentityCreator<T>
    {
        private readonly IIdentityProvider _provider;
        /// <summary>
        /// Constructs an identity creator for type T.
        /// </summary>
        /// <param name="provider">The identity provider that will actually create the identity values.</param>
        public IdentityCreator(IIdentityProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Create an identity value for a type T.
        /// </summary>
        /// <typeparam name="K">The underlying type of the identity value.</typeparam>
        /// <param name="value">The underlying value of the identity value.</param>
        /// <returns>A newly constructed identity value for type T with the specified underlying value.
        /// May return null if the provider cannot construct the identity value.</returns>
        public IIdentity<T> Create<K>(K value)
            => _provider.Create<T, K>(value);

        IIdentity IIdentityCreator.Create<K>(K value)
            => Create(value);
    }
    /// <summary>
    /// A utility identity creator that takes an IIdentityProvider reference and a type.
    /// </summary>
    public struct IdentityCreator : IIdentityCreator
    {
        private readonly IIdentityProvider _provider;
        private readonly Type _type;

        /// <summary>
        /// Constructs an identity creator for some type.
        /// </summary>
        /// <param name="provider">The identity provider that will actually create the identity values.</param>
        /// <param name="type">The type of object the created identity values refer to.</param>
        public IdentityCreator(IIdentityProvider provider, Type type)
        {
            _provider = provider;
            _type = type;
        }
        /// <summary>
        /// Create an identity value for the registered type.
        /// </summary>
        /// <typeparam name="K">The underlying type of the identity value.</typeparam>
        /// <param name="value">The underlying value of the identity value.</param>
        /// <returns>A newly constructed identity value for the registered type with the specified underlying value.
        /// May return null if the provider cannot construct the identity value.</returns>
        public IIdentity Create<K>(K value)
            => _provider.Create(_type, value);
    }
}
