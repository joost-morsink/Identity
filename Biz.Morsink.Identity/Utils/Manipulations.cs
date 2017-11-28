using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.Utils
{
    /// <summary>
    /// Manipulation functions for identity values
    /// </summary>
    public static class Manipulations
    {
        #region Helper structs
        /// <summary>
        /// Helper struct for morphing identity values.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        public struct IdentityMorph<T>
        {
            private readonly IIdentity<T> id;

            internal IdentityMorph(IIdentity<T> id)
            {
                this.id = id;
            }
            /// <summary>
            /// Morphs the identity value to represent another type.
            /// </summary>
            /// <typeparam name="R">The last entity component typw.</typeparam>
            /// <returns>A new identity if the morph was succesful, null otherwise.</returns>
            public IIdentity<R> To<R>()
                => id.Provider.Creator<R>().Create(id.Value);
            /// <summary>
            /// Tries to morph the identity to represent another type.
            /// </summary>
            /// <typeparam name="R">The last entity component type.</typeparam>
            /// <param name="identity">The new identity value.</param>
            /// <returns>True if the morph was successful, false otherwise.</returns>
            public bool TryTo<R>(out IIdentity<R> identity)
            {
                identity = To<R>();
                return identity != null;
            }
        }
        /// <summary>
        /// Helper struct for morphing identity values.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        public struct IdentityMorph<T, U>
        {
            private readonly IIdentity<T, U> id;

            internal IdentityMorph(IIdentity<T, U> id)
            {
                this.id = id;
            }
            /// <summary>
            /// Morphs the identity value to represent another type.
            /// </summary>
            /// <typeparam name="R">The last entity component typw.</typeparam>
            /// <returns>A new identity if the morph was succesful, null otherwise.</returns>
            public IIdentity<T, R> To<R>()
                => id.Provider.Creator<R>().Create(id.Value) as IIdentity<T, R>;
            /// <summary>
            /// Tries to morph the identity to represent another type.
            /// </summary>
            /// <typeparam name="R">The last entity component type.</typeparam>
            /// <param name="identity">The new identity value.</param>
            /// <returns>True if the morph was successful, false otherwise.</returns>
            public bool TryTo<R>(out IIdentity<T, R> identity)
            {
                identity = To<R>();
                return identity != null;
            }
        }
        /// <summary>
        /// Helper struct for morphing identity values.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        /// <typeparam name="V">The third entity component type.</typeparam>
        public struct IdentityMorph<T, U, V>
        {
            private readonly IIdentity<T, U, V> id;

            internal IdentityMorph(IIdentity<T, U, V> id)
            {
                this.id = id;
            }
            /// <summary>
            /// Morphs the identity value to represent another type.
            /// </summary>
            /// <typeparam name="R">The last entity component typw.</typeparam>
            /// <returns>A new identity if the morph was succesful, null otherwise.</returns>
            public IIdentity<T, U, R> To<R>()
                => id.Provider.Creator<R>().Create(id.Value) as IIdentity<T, U, R>;
            /// <summary>
            /// Tries to morph the identity to represent another type.
            /// </summary>
            /// <typeparam name="R">The last entity component type.</typeparam>
            /// <param name="identity">The new identity value.</param>
            /// <returns>True if the morph was successful, false otherwise.</returns>
            public bool TryTo<R>(out IIdentity<T, U, R> identity)
            {
                identity = To<R>();
                return identity != null;
            }
        }
        /// <summary>
        /// Helper struct for morphing identity values.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        /// <typeparam name="V">The third entity component type.</typeparam>
        /// <typeparam name="W">The fourth entity component type.</typeparam>
        public struct IdentityMorph<T, U, V, W>
        {
            private readonly IIdentity<T, U, V, W> id;

            internal IdentityMorph(IIdentity<T, U, V, W> id)
            {
                this.id = id;
            }
            /// <summary>
            /// Morphs the identity value to represent another type.
            /// </summary>
            /// <typeparam name="R">The last entity component typw.</typeparam>
            /// <returns>A new identity if the morph was succesful, null otherwise.</returns>
            public IIdentity<T, U, V, R> To<R>()
                => id.Provider.Creator<R>().Create(id.Value) as IIdentity<T, U, V, R>;
            /// <summary>
            /// Tries to morph the identity to represent another type.
            /// </summary>
            /// <typeparam name="R">The last entity component type.</typeparam>
            /// <param name="identity">The new identity value.</param>
            /// <returns>True if the morph was successful, false otherwise.</returns>
            public bool TryTo<R>(out IIdentity<T, U, V, R> identity)
            {
                identity = To<R>();
                return identity != null;
            }
        }
        /// <summary>
        /// Helper struct for morphing identity values.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        /// <typeparam name="V">The third entity component type.</typeparam>
        /// <typeparam name="W">The fourth entity component type.</typeparam>
        /// <typeparam name="X">The fifth entity component type.</typeparam>
        public struct IdentityMorph<T, U, V, W, X>
        {
            private readonly IIdentity<T, U, V, W, X> id;

            internal IdentityMorph(IIdentity<T, U, V, W, X> id)
            {
                this.id = id;
            }
            /// <summary>
            /// Morphs the identity value to represent another type.
            /// </summary>
            /// <typeparam name="R">The last entity component typw.</typeparam>
            /// <returns>A new identity if the morph was succesful, null otherwise.</returns>
            public IIdentity<T, U, V, W, R> To<R>()
                => id.Provider.Creator<R>().Create(id.Value) as IIdentity<T, U, V, W, R>;
            /// <summary>
            /// Tries to morph the identity to represent another type.
            /// </summary>
            /// <typeparam name="R">The last entity component type.</typeparam>
            /// <param name="identity">The new identity value.</param>
            /// <returns>True if the morph was successful, false otherwise.</returns>
            public bool TryTo<R>(out IIdentity<T, U, V, W, R> identity)
            {
                identity = To<R>();
                return identity != null;
            }
        }
        /// <summary>
        /// Helper struct for extending identity values.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        public struct IdentityExtend<T>
        {
            private readonly IIdentity<T> id;

            internal IdentityExtend(IIdentity<T> id)
            {
                this.id = id;
            }
            /// <summary>
            /// Extends the identity value with a new component.
            /// </summary>
            /// <typeparam name="R">The entity component type of the extension.</typeparam>
            /// <param name="val">The component value for the extension.</param>
            /// <returns>An extended identity value if successful, null otherwise.</returns>
            public IIdentity<T, R> To<R>(object val)
                => id.Provider.Creator<R>().Create((id.ComponentValue, val)) as IIdentity<T, R>;
            /// <summary>
            /// Tries to extend the identity value with a new component.
            /// </summary>
            /// <typeparam name="R">The entity component type of the extension.</typeparam>
            /// <param name="val">The component value for the extension.</param>
            /// <param name="identity">The new identity value.</param>
            /// <returns>True if conversion is successful, false otherwise.</returns>
            public bool TryTo<R>(object val, out IIdentity<T, R> identity)
            {
                identity = To<R>(val);
                return identity != null;
            }
        }
        /// <summary>
        /// Helper struct for extending identity values.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        public struct IdentityExtend<T, U>
        {
            private readonly IIdentity<T, U> id;

            internal IdentityExtend(IIdentity<T, U> id)
            {
                this.id = id;
            }
            /// <summary>
            /// Extends the identity value with a new component.
            /// </summary>
            /// <typeparam name="R">The entity component type of the extension.</typeparam>
            /// <param name="val">The component value for the extension.</param>
            /// <returns>An extended identity value if succesful, null otherwise.</returns>
            public IIdentity<T, U, R> To<R>(object val)
                => id.Provider.Creator<R>().Create((id.Parent.ComponentValue, id.ComponentValue, val)) as IIdentity<T, U, R>;
            /// <summary>
            /// Tries to extend the identity value with a new component.
            /// </summary>
            /// <typeparam name="R">The entity component type of the extension.</typeparam>
            /// <param name="val">The component value for the extension.</param>
            /// <param name="identity">The new identity value.</param>
            /// <returns>True if conversion is successful, false otherwise.</returns>
            public bool TryTo<R>(object val, out IIdentity<T, U, R> identity)
            {
                identity = To<R>(val);
                return identity != null;
            }
        }
        /// <summary>
        /// Helper struct for extending identity values.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        /// <typeparam name="V">The third entity component type.</typeparam>
        public struct IdentityExtend<T, U, V>
        {
            private readonly IIdentity<T, U, V> id;

            internal IdentityExtend(IIdentity<T, U, V> id)
            {
                this.id = id;
            }
            /// <summary>
            /// Extends the identity value with a new component.
            /// </summary>
            /// <typeparam name="R">The entity component type of the extension.</typeparam>
            /// <param name="val">The component value for the extension.</param>
            /// <returns>An extended identity value if succesful, null otherwise.</returns>
            public IIdentity<T, U, V, R> To<R>(object val)
                => id.Provider.Creator<R>().Create((id.Parent.Parent.ComponentValue, id.Parent.ComponentValue, id.ComponentValue, val)) as IIdentity<T, U, V, R>;
            /// <summary>
            /// Tries to extend the identity value with a new component.
            /// </summary>
            /// <typeparam name="R">The entity component type of the extension.</typeparam>
            /// <param name="val">The component value for the extension.</param>
            /// <param name="identity">The new identity value.</param>
            /// <returns>True if conversion is successful, false otherwise.</returns>
            public bool TryTo<R>(object val, out IIdentity<T, U, V, R> identity)
            {
                identity = To<R>(val);
                return identity != null;
            }
        }
        /// <summary>
        /// Helper struct for extending identity values.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        /// <typeparam name="V">The third entity component type.</typeparam>
        /// <typeparam name="W">The fourth entity component type.</typeparam>
        public struct IdentityExtend<T, U, V, W>
        {
            private readonly IIdentity<T, U, V, W> id;

            internal IdentityExtend(IIdentity<T, U, V, W> id)
            {
                this.id = id;
            }
            /// <summary>
            /// Extends the identity value with a new component.
            /// </summary>
            /// <typeparam name="R">The entity component type of the extension.</typeparam>
            /// <param name="val">The component value for the extension.</param>
            /// <returns>An extended identity value if succesful, null otherwise.</returns>
            public IIdentity<T, U, V, W, R> To<R>(object val)
                => id.Provider.Creator<R>().Create((id.Parent.Parent.Parent.ComponentValue, id.Parent.Parent.ComponentValue, id.Parent.ComponentValue, id.ComponentValue, val)) as IIdentity<T, U, V, W, R>;
            /// <summary>
            /// Tries to extend the identity value with a new component.
            /// </summary>
            /// <typeparam name="R">The entity component type of the extension.</typeparam>
            /// <param name="val">The component value for the extension.</param>
            /// <param name="identity">The new identity value.</param>
            /// <returns>True if conversion is successful, false otherwise.</returns>
            public bool TryTo<R>(object val, out IIdentity<T, U, V, W, R> identity)
            {
                identity = To<R>(val);
                return identity != null;
            }
        }
        #endregion

        /// <summary>
        /// Sets up to morph the identity value.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <param name="baseId">The identity value to be morphed.</param>
        /// <returns>A helper struct to morph identity values.</returns>
        public static IdentityMorph<T> Morph<T>(this IIdentity<T> baseId)
            => new IdentityMorph<T>(baseId);
        /// <summary>
        /// Sets up to morph the identity value.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        /// <param name="baseId">The identity value to be morphed.</param>
        /// <returns>A helper struct to morph identity values.</returns>
        public static IdentityMorph<T, U> Morph<T, U>(this IIdentity<T, U> baseId)
            => new IdentityMorph<T, U>(baseId);
        /// <summary>
        /// Sets up to morph the identity value.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        /// <typeparam name="V">The third entity component type.</typeparam>
        /// <param name="baseId">The identity value to be morphed.</param>
        /// <returns>A helper struct to morph identity values.</returns>
        public static IdentityMorph<T, U, V> Morph<T, U, V>(this IIdentity<T, U, V> baseId)
            => new IdentityMorph<T, U, V>(baseId);
        /// <summary>
        /// Sets up to morph the identity value.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        /// <typeparam name="V">The third entity component type.</typeparam>
        /// <typeparam name="W">The fourth entity component type.</typeparam>
        /// <param name="baseId">The identity value to be morphed.</param>
        /// <returns>A helper struct to morph identity values.</returns>
        public static IdentityMorph<T, U, V, W> Morph<T, U, V, W>(this IIdentity<T, U, V, W> baseId)
            => new IdentityMorph<T, U, V, W>(baseId);
        /// <summary>
        /// Sets up to morph the identity value.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        /// <typeparam name="V">The third entity component type.</typeparam>
        /// <typeparam name="W">The fourth entity component type.</typeparam>
        /// <typeparam name="X">The fifth entity component type.</typeparam>
        /// <param name="baseId">The identity value to be morphed.</param>
        /// <returns>A helper struct to morph identity values.</returns>
        public static IdentityMorph<T, U, V, W, X> Morph<T, U, V, W, X>(this IIdentity<T, U, V, W, X> baseId)
            => new IdentityMorph<T, U, V, W, X>(baseId);
        /// <summary>
        /// Sets up to extend the identity value.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <param name="baseId">The identity value to be extended.</param>
        /// <returns>A helper struct to extend identity values.</returns>
        public static IdentityExtend<T> Extend<T>(this IIdentity<T> baseId)
            => new IdentityExtend<T>(baseId);
        /// <summary>
        /// Sets up to extend the identity value.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        /// <param name="baseId">The identity value to be extended.</param>
        /// <returns>A helper struct to extend identity values.</returns>
        public static IdentityExtend<T, U> Extend<T, U>(this IIdentity<T, U> baseId)
            => new IdentityExtend<T, U>(baseId);
        /// <summary>
        /// Sets up to extend the identity value.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        /// <typeparam name="V">The third entity component type.</typeparam>
        /// <param name="baseId">The identity value to be extended.</param>
        /// <returns>A helper struct to extend identity values.</returns>
        public static IdentityExtend<T, U, V> Extend<T, U, V>(this IIdentity<T, U, V> baseId)
            => new IdentityExtend<T, U, V>(baseId);
        /// <summary>
        /// Sets up to extend the identity value.
        /// </summary>
        /// <typeparam name="T">The first entity component type.</typeparam>
        /// <typeparam name="U">The second entity component type.</typeparam>
        /// <typeparam name="V">The third entity component type.</typeparam>
        /// <typeparam name="W">The fourth entity component type.</typeparam>
        /// <param name="baseId">The identity value to be extended.</param>
        /// <returns>A helper struct to extend identity values.</returns>
        public static IdentityExtend<T, U, V, W> Extend<T, U, V, W>(this IIdentity<T, U, V, W> baseId)
            => new IdentityExtend<T, U, V, W>(baseId);

    }
}
