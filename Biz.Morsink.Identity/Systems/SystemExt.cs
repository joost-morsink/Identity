using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.Systems
{
    /// <summary>
    /// Extension methods fpr system decorated identity values.
    /// </summary>
    public static class SystemExt
    {
        /// <summary>
        /// Assigns a system to the identity value.
        /// </summary>
        /// <param name="id">The identity value.</param>
        /// <param name="sys">The system identity value.</param>
        /// <returns>An identity value with added system identity.</returns>
        public static IIdentity WithSystem(this IIdentity id, IIdentity<Sys> sys)
            => new IdentityWithSystem<IIdentity>(sys, id);
        /// <summary>
        /// Assigns a system to the identity value.
        /// <typeparam name="T">The entity type of the identity value.</typeparam>
        /// <param name="id">The identity value.</param>
        /// <param name="sys">The system identity value.</param>
        /// <returns>An identity value with added system identity.</returns>
        public static IIdentity<T> WithSystem<T>(this IIdentity<T> id, IIdentity<Sys> sys)
            => new IdentityWithSystem<T, IIdentity<T>>(sys, id);
    }
}
