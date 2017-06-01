using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity.Utils
{
    /// <summary>
    /// Utility class for identity values.
    /// This class is meant to be used by Identity Providers and not directly by client code.
    /// </summary>
    public class IdentityUtils
    {
        private static Type[] IDENTITY_TYPES = new[]
        {
            null,
            typeof(Identity<,>),
            typeof(Identity<,,,>),
            typeof(Identity<,,,,,>)
        };
        /// <summary>
        /// Creates a new identity value based on reflection mechanisms.
        /// Might be slow.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="provider">The provider</param>
        /// <param name="types">The types of the identity components.</param>
        /// <param name="vals">The component values.</param>
        /// <returns></returns>
        public static IIdentity<T> Create<T>(IIdentityProvider provider, Type[] types, object[] vals)
        {
            if (types.Length != vals.Length)
                throw new ArgumentException("types.Length should equal vals.Length");
            var type = IDENTITY_TYPES[types.Length].MakeGenericType(types.Concat(vals.Select(v => v.GetType())).ToArray());
            return (IIdentity<T>)Activator.CreateInstance(type, new object[] { provider }.Concat(vals).ToArray());
        }
    }
}
