using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// This class implements a decorated identity value, composed of the actual identity value and a decoration.
    /// </summary>
    /// <typeparam name="D">The type of decoration.</typeparam>
    /// <typeparam name="I">The type of the identity value.</typeparam>
    /// <typeparam name="T">The entity type of the identity value.</typeparam>
    public class DecoratedIdentity<D, I, T> : IIdentity<T>, IIdentityContainer
        where I : IIdentity<T>
        where D : IIdentity
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="decoration">The decoration value.</param>
        /// <param name="identity">The identity value.</param>
        public DecoratedIdentity(D decoration, I identity)
        {
            Decoration = decoration;
            Identity = identity;
        }

        /// <summary>
        /// Gets the decoration value.
        /// </summary>
        public D Decoration { get; }
        /// <summary>
        /// Gets the identity value.
        /// </summary>
        public I Identity { get; }

        /// <summary>
        /// The Provider is the Identity's Provider.
        /// </summary>
        public IIdentityProvider Provider => Identity.Provider;

        /// <summary>
        /// The ForType is typeof(T).
        /// </summary>
        public Type ForType => typeof(T);

        /// <summary>
        /// The Value is the Identity's Value.
        /// </summary>
        public object Value => Identity.Value;

        /// <summary>
        /// The ComponentValue is the Identity's ComponentValue.
        /// </summary>
        public object ComponentValue => Identity.ComponentValue;

        /// <summary>
        /// The Identities are the Identity's Identities concatenated with those of the Decoration.
        /// </summary>
        public IEnumerable<IIdentity> Identities => Identity.Identities.Concat(Decoration.Identities);

        /// <summary>
        /// The Arity is the Identity's Arity.
        /// </summary>
        public int Arity => Identity.Arity;
        
        public override int GetHashCode()
            => Identity.GetHashCode();
        public override bool Equals(object obj) => Equals(obj as IIdentity<T>);
        public bool Equals(IIdentity other) => Equals(other as IIdentity<T>);

        public bool Equals(IIdentity<T> other)
            => Identity.Equals(other);

        /// <summary>
        /// Maps an identity value into the current decoration.
        /// </summary>
        /// <param name="id">The identity value to map.</param>
        /// <returns>A decorated identity value corresponding to the id parameter.</returns>
        public IIdentity Map(IIdentity id)
            => (IIdentity)Activator.CreateInstance(typeof(DecoratedIdentity<,>).MakeGenericType(typeof(D), id.ForType), Decoration, id);
        /// <summary>
        /// Maps an identity value into the current decoration.
        /// </summary>
        /// <typeparam name="U"></typeparam>
        /// <param name="id">The identity value to map.</param>
        /// <returns>A decorated identity value corresponding to the id parameter.</returns>        
        public IIdentity<U> Map<U>(IIdentity<U> id)
            => new DecoratedIdentity<D, IIdentity<U>, U>(Decoration, id);
    }
    /// <summary>
    /// This component implements a decorated identity value, composed of the actual identity value and a decoration.
    /// </summary>
    /// <typeparam name="D">The type of decoration.</typeparam>
    /// <typeparam name="T">The entity type of the identity value.</typeparam>
    public class DecoratedIdentity<D, T> : DecoratedIdentity<D, IIdentity<T>, T>
        where D : IIdentity
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="decoration">The decoration value.</param>
        /// <param name="identity">The identity value.</param>
        public DecoratedIdentity(D decoration, IIdentity<T> identity) : base(decoration, identity) { }
    }
}
