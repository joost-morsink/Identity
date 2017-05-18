using Biz.Morsink.DataConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity
{
    public static class FreeIdentity
    {
        public struct FreeIdentityCreator<T>
        {
            private readonly IIdentity<T> _id;

            internal FreeIdentityCreator(IIdentity<T> id)
            {
                _id = id;
            }
            public FreeIdentity<T, K> WithType<K>()
            {
                var conv = _id.Provider.GetConverter(typeof(T), false);
                return conv.Convert(_id.Value).TryTo(out K cres)
                    ? FreeIdentity<T>.Create(cres)
                    : null;
            }
        }
        public static FreeIdentityCreator<T> MakeFree<T>(this IIdentity<T> id)
            => new FreeIdentityCreator<T>(id);
    }
    /// <summary>
    /// Helper class to create 'free' identity values. 
    /// </summary>
    /// <typeparam name="T">The entity type the identity value will refer to.</typeparam>
    public static class FreeIdentity<T>
    {
        /// <summary>
        /// Creates a parentless FreeIdentity.
        /// </summary>
        /// <typeparam name="K">The type of the underlying value.</typeparam>
        /// <param name="componentValue">The underlying value.</param>
        /// <returns>A FreeIdentity value.</returns>
        public static FreeIdentity<T, K> Create<K>(K componentValue)
            => new FreeIdentity<T, K>(null, componentValue);
        /// <summary>
        /// Creates a FreeIdentity with a parent.
        /// </summary>
        /// <typeparam name="K">The type of the underlying value.</typeparam>
        /// <param name="parent">The parent identity for this identity.</param>
        /// <param name="componentValue">The underlying component value.</param>
        /// <returns>A FreeIdentity value.</returns>
        public static FreeIdentity<T, K> Create<K>(IIdentity parent, K componentValue)
            => new FreeIdentity<T, K>(parent, componentValue);
    }
    /// <summary>
    /// This class represents a 'free' identity value.
    /// A free identity value is an identity value not bound to a certain identity provider. 
    /// It is also dynamic with respect to a optional parent identity value.
    /// </summary>
    /// <typeparam name="T">The entity type the identity value will refer to.</typeparam>
    /// <typeparam name="K">The type of the underlying component value for this identity value.</typeparam>
    public class FreeIdentity<T, K> : IMultiaryIdentity<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parent">An optional parent identity value.</param>
        /// <param name="componentValue">The component value.</param>
        public FreeIdentity(IIdentity parent, K componentValue)
        {
            Parent = parent;
            ComponentValue = componentValue;
        }
        /// <summary>
        /// Gets the parent identity.
        /// </summary>
        public IIdentity Parent { get; }
        /// <summary>
        /// Gets the component value.
        /// </summary>
        public K ComponentValue { get; }
        object IIdentity.ComponentValue => ComponentValue;
        /// <summary>
        /// A FreeIdentity's provider is always null.
        /// </summary>
        public IIdentityProvider Provider => null;
        /// <summary>
        /// The entity type this identity value refers to.
        /// </summary>
        public Type ForType => typeof(T);

        /// <summary>
        /// The value is an enumerable of component values obtained from a parent identity concatenated with this identity's component value.
        /// </summary>
        public IEnumerable<object> Value => Identities.Reverse().Select(id => id.ComponentValue);
        object IIdentity.Value => Parent == null ? (object)ComponentValue : Value;

        /// <summary>
        /// Gets all the identities that are contained in this identity value.
        /// </summary>
        public IEnumerable<IIdentity> Identities
        {
            get
            {
                yield return this;
                if (Parent != null)
                    foreach (var id in Parent.Identities)
                        yield return id;
            }
        }
        /// <summary>
        /// The arity is always 1 more than the arity of its parent.
        /// If there is no parent, the arity is 1.
        /// </summary>
        public int Arity => 1 + (Parent?.Arity ?? 0);
    }
}
