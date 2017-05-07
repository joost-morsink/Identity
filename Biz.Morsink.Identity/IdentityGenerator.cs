using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// Utility struct for generation of identity values for a certain identity provider and type.
    /// </summary>
    /// <typeparam name="T">The type of entity the identity values refer to.</typeparam>
    public struct IdentityGenerator<T> : IIdentityGenerator<T>
    {
        private readonly IIdentityProvider _provider;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">The identity provider associated with the generated identity values.</param>
        public IdentityGenerator(IIdentityProvider provider)
        {
            _provider = provider;
        }
        /// <summary>
        /// Gets the type identity values are created for.
        /// </summary>
        public Type ForType => typeof(T);
        /// <summary>
        /// Gets a new identity value for an entity.
        /// </summary>
        /// <param name="entity">The entity to get a new identity value for.</param>
        /// <returns>A new and unique identity value.</returns>
        public IIdentity<T> New(T entity)
            => _provider.New(entity);

        IIdentity IIdentityGenerator.New(object entity)
            => New((T)entity);
    }
    /// <summary>
    /// Utility struct for generation of identity values for a certain identity provider and type.
    /// </summary>
    public struct IdentityGenerator : IIdentityGenerator
    {
        private readonly IIdentityProvider _provider;
        private readonly Type _type;
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">The identity provider associated with the generated identity values.</param>
        /// <param name="type">The type of entity the identity values will refer to.</param>
        public IdentityGenerator(IIdentityProvider provider, Type type)
        {
            _provider = provider;
            _type = type;
        }
        /// <summary>
        /// Gets the type identity values are created for.
        /// </summary>
        public Type ForType => _type;
        /// <summary>
        /// Gets a new identity value for an entity.
        /// </summary>
        /// <param name="entity">The entity to get a new identity value for.</param>
        /// <returns>A new and unique identity value.</returns>
        public IIdentity New(object entity)
            => _provider.New(_type, entity);
    }
}
