using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity
{
    public static class Utility
    {
        /// <summary>
        /// Constructs an identity creator.
        /// </summary>
        /// <typeparam name="T">The type of object the identity values will refer to.</typeparam>
        /// <param name="provider">The provider that will actually create the identity values.</param>
        /// <returns>An identity creator.</returns>
        public static IdentityCreator<T> Creator<T>(this IIdentityProvider provider)
            => new IdentityCreator<T>(provider);
        /// <summary>
        /// Constructs an identity creator.
        /// </summary>
        /// <param name="provider">The provider that will actually create the identity values.</param>
        /// <param name="type">The type of object the identity values will refer to.</param>
        /// <returns>An identity creator.</returns>
        public static IdentityCreator Creator(this IIdentityProvider provider, Type type)
            => new IdentityCreator(provider, type);
        /// <summary>
        /// Tries to find an identity value for a certain type in the parent chain.
        /// </summary>
        /// <typeparam name="T">The type of object the identity value refers to.</typeparam>
        /// <param name="id">The identity value to search the parent chain of.</param>
        /// <returns>An identity value if one could be found for T, null otherwise.</returns>
        public static IIdentity<T> For<T>(this IIdentity id)
            => id.Identities.OfType<IIdentity<T>>().FirstOrDefault();
    }
}
