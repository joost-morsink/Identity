using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// This interface exposes a method to create identity values for entities.
    /// </summary>
    public interface IIdentityGenerator
    {
        /// <summary>
        /// Gets the type identity values are created for.
        /// </summary>
        Type ForType { get; }
        /// <summary>
        /// Gets a new identity value for an entity.
        /// </summary>
        /// <param name="entity">The entity to get a new identity value for.</param>
        /// <returns>A new and unique identity value.</returns>
        IIdentity New(object entity);
    }
    /// <summary>
    /// Indicates whether this identity provider supports the generation of new identity values for new entities.
    /// </summary>
    /// <typeparam name="T">The type of entity the identity values are created for.</typeparam>
    public interface IIdentityGenerator<T> : IIdentityGenerator
    {
        /// <summary>
        /// Gets a new identity value for an entity.
        /// </summary>
        /// <param name="entity">The entity to get a new identity value for.</param>
        /// <returns>A new and unique identity value.</returns>
        IIdentity<T> New(T entity);
    }
}
