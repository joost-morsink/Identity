using Biz.Morsink.DataConvert;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{ 
    /// <summary>
    /// This interface describes an identity provider. 
    /// All instances of identity values should be created through instances of providers.
    /// </summary>
    public interface IIdentityProvider : IEqualityComparer<IIdentity>
    {
        /// <summary>
        /// Tries to translate some identity value to one that is owned by this identity provider.
        /// </summary>
        /// <param name="id">The externally owned identity value.</param>
        /// <returns>If the provider is able to understand the incoming identity value, a newly constructed identity value.
        /// If the identity value cannot be translated, null.</returns>
        IIdentity Translate(IIdentity id);
        /// <summary>
        /// Tries to translate some identity value to one that is owned by this identity provider.
        /// </summary>
        /// <typeparam name="T">The type the identity value refers to.</typeparam>
        /// <param name="id">The externally owned identity value.</param>
        /// <returns>If the provider is able to understand the incoming identity value, a newly constructed identity value.
        /// If the identity value cannot be translated, null.</returns>
        IIdentity<T> Translate<T>(IIdentity<T> id);
        /// <summary>
        /// Gets a data converter for converting underlying identity values.
        /// </summary>
        /// <param name="t">The type of object the identity value refers to.</param>
        /// <param name="incoming">Indicates if the converter should handle incoming or outgoing conversions.</param>
        /// <returns>An IDataConverter that is able to make conversions between different types of values.</returns>
        IDataConverter GetConverter(Type t, bool incoming);
        /// <summary>
        /// Creates an identity value for a certain type of a certain value type.
        /// </summary>
        /// <typeparam name="K">The type of the underlying value.</typeparam>
        /// <param name="forType">The type of object the identity value refers to.</param>
        /// <param name="value">The underlying value.</param>
        /// <returns>If the provider is able to construct an identity value for the specified parameters, the identity value. Null otherwise.</returns>
        IIdentity Create<K>(Type forType, K value);
        /// <summary>
        /// Creates an identity value for a certain type of a certain value type.
        /// </summary>
        /// <typeparam name="T">The type of object the identity value refers to.</typeparam>
        /// <typeparam name="K">The type of the underlying value.</typeparam>
        /// <param name="value">The underlying value.</param>
        /// <returns>If the provider is able to construct an identity value for the specified parameters, the identity value. Null otherwise.</returns>
        IIdentity<T> Create<T, K>(K value);
        /// <summary>
        /// Indicates whether this identity provider supports the generation of new identity values for new entities.
        /// </summary>
        bool SupportsNewIdentities { get; }
        /// <summary>
        /// Creates a new identity value for some entity of some type.
        /// </summary>
        /// <param name="forType">The type of entity the identity value is created for.</param>
        /// <param name="entity">The entity the identity value is created for.</param>
        /// <returns>A new and unique identity value for the entity.</returns>
        /// <exception cref="InvalidOperationException">When SupportsNewIdentities is false.</exception>
        IIdentity New(Type forType, object entity);
        /// <summary>
        /// Creates a new identity value for some entity of some type.
        /// </summary>
        /// <typeparam name="T">The type of entity the identity value is created for.</typeparam>
        /// <param name="entity">The entity the identity value is created for.</param>
        /// <returns>A new and unique identity value for the entity.</returns>
        /// <exception cref="InvalidOperationException">When SupportsNewIdentities is false.</exception>
        IIdentity<T> New<T>(T entity);
    }
}
