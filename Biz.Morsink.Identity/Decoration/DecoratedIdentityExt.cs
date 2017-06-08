using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.Decoration
{
    /// <summary>
    /// Extension method class.
    /// </summary>
    public static class DecoratedIdentityExt
    {
        /// <summary>
        /// Decorates an identity value
        /// </summary>
        /// <typeparam name="D">The type of the decoration.</typeparam>
        /// <typeparam name="T">The entity type of the identity value.</typeparam>
        /// <param name="id">The identity value to be decorated.</param>
        /// <param name="decoration">The decoration value.</param>
        /// <returns>A decorated identity value.</returns>
        public static DecoratedIdentity<D, T> Decorate<D, T>(this IIdentity<T> id, D decoration)
            where D : IIdentity
            => new DecoratedIdentity<D, T>(decoration, id);
    }

}
