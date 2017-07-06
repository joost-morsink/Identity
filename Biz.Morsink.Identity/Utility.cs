using Biz.Morsink.DataConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// Utility class for anything identity related.
    /// Contains extension methods.
    /// </summary>
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

        /// <summary>
        /// Constructs an identity generator for a certain type of entity.
        /// </summary>
        /// <typeparam name="T">The type of entity the identity value should refer to.</typeparam>
        /// <param name="provider">The provider to use for identity value generation.</param>
        /// <returns>An identity generator.</returns>
        public static IdentityGenerator<T> Generator<T>(this IIdentityProvider provider)
            => new IdentityGenerator<T>(provider);

        /// <summary>
        /// Constructs an identity generator for a certain type of entity.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="provider">The provider to use for identity value generation.</param>
        /// <returns>An identity generator.</returns>
        public static IdentityGenerator Generator(this IIdentityProvider provider, Type type)
            => new IdentityGenerator(provider, type);

        private static R Call<T, R>(this T t, Func<T, R> f)
            => f(t);
        /// <summary>
        /// Replaces a type of converter in an enumerable of converters.
        /// </summary>
        /// <typeparam name="T">The type of converter.</typeparam>
        /// <param name="converters">A pipeline of converters.</param>
        /// <param name="replacer">A replacer function.</param>
        /// <returns>A new enumerable of converters with the specified type of converter replaced.</returns>
        public static IEnumerable<IConverter> Replace<T>(this IEnumerable<IConverter> converters, Func<T, IConverter> replacer)
            where T : class, IConverter
            => converters.Select(c => (c as T)?.Call(replacer) ?? c).Where(c => c != null);
        /// <summary>
        /// Indicates whether the identity provider supports identity values for the entity type.
        /// </summary>
        /// <returns>True if the entity type is supported.</returns>
        /// <param name="provider">The identity provider.</param>
        /// <typeparam name="T">The entity type.</typeparam>
        public static bool Supports<T>(this IIdentityProvider provider)
            => provider.GetUnderlyingType(typeof(T)) != null;
        /// <summary>
        /// Indicates whether the identity provider supports identity values for the entity type.
        /// </summary>
        /// <returns>True if the entity type is supported.</returns>
        /// <param name="provider">The identity provider.</param>
        /// <param name="type">The entity type.</param>
        public static bool Supports(this IIdentityProvider provider, Type type)
            => provider.GetUnderlyingType(type) != null;
    }
}