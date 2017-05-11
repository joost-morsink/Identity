using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// Reference implementation for 1-ary indentity values.
    /// </summary>
    /// <typeparam name="Type1">The type of object an instance is an identity value for.</typeparam>
    /// <typeparam name="Key1">The type of the identity value.</typeparam>
    public class Identity<Type1, Key1> : IIdentity<Type1>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">A reference to the provider of this instance.</param>
        /// <param name="value">The underlying value.</param>
        public Identity(IIdentityProvider provider, Key1 value)
        {
            Provider = provider;
            Value = value;
        }
        /// <summary>
        /// The underlying value of the identity value.
        /// </summary>
        public Key1 Value { get; }
        /// <summary>
        /// For unary identity values the component value is equal to the Value.
        /// </summary>
        public Key1 ComponentValue => Value;

        object IIdentity<Type1>.ComponentValue => Value;

        /// <summary>
        /// Gets the provider this instance belongs to.
        /// </summary>
        public IIdentityProvider Provider { get; }

        /// <summary>
        /// Gets the type of object the identity value refers to.
        /// </summary>
        public Type ForType => typeof(Type1);

        object IIdentity.Value => Value;

        /// <summary>
        /// For unary identity values, this enumerable contains only a self reference.
        /// </summary>
        public IEnumerable<IIdentity> Identities
        {
            get { yield return this; }
        }
        /// <summary>
        /// The arity of unary identity values is always 1.
        /// </summary>
        public int Arity => 1;
    }
    /// <summary>
    /// Reference implementation for 2-ary identity values.
    /// </summary>
    /// <typeparam name="Type1">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="Type2">The type of object an instance is primarily an identity value for.</typeparam>
    /// <typeparam name="Key1">The type of the identity component value corresponding to Type1.</typeparam>
    /// <typeparam name="Key2">The type of the identity component value corresponding to Type2.</typeparam>
    public class Identity<Type1, Type2, Key1, Key2> : IIdentity<Type1, Type2>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">A reference to the provider of this instance.</param>
        /// <param name="value1">The underlying first component value.</param>
        /// <param name="value2">The underlying second component value.</param>
        public Identity(IIdentityProvider provider, Key1 value1, Key2 value2)
            : this(new Identity<Type1, Key1>(provider, value1), value2)
        { }
        /// <summary>
        /// Constuctor.
        /// </summary>
        /// <param name="parent">The parent identity.</param>
        /// <param name="value">This identity's component value.</param>
        public Identity(Identity<Type1, Key1> parent, Key2 value)
        {
            Provider = parent.Provider;
            Parent = parent;
            ComponentValue = value;
        }

        /// <summary>
        /// Get the identity value's underlying value.
        /// Binary identity values contain a 2-tuple of component values.
        /// </summary>
        public (Key1, Key2) Value => (Parent.ComponentValue, ComponentValue);

        /// <summary>
        /// Gets the binary identity value's second component value.
        /// </summary>
        public Key2 ComponentValue { get; }

        /// <summary>
        /// Gets the identity value's parent.
        /// </summary>
        public Identity<Type1, Key1> Parent { get; }

        IIdentity IMultiaryIdentity.Parent => Parent;
        IIdentity<Type1> IMultiaryIdentity<Type2, IIdentity<Type1>>.Parent => Parent;

        object IIdentity<Type2>.ComponentValue => ComponentValue;

        /// <summary>
        /// Gets the provider this instance belongs to.
        /// </summary>
        public IIdentityProvider Provider { get; }

        /// <summary>
        /// Gets the type of object the identity value refers to.
        /// </summary>
        public Type ForType => typeof(Type2);

        object IIdentity.Value => Value;

        /// <summary>
        /// Gets the parent chain of identity values. 
        /// Binary identity values return a self reference and the direct parent.
        /// </summary>
        public IEnumerable<IIdentity> Identities
        {
            get
            {
                yield return this;
                yield return Parent;
            }
        }
        /// <summary>
        /// The arity of binary identity values is always 2.
        /// </summary>
        public int Arity => 2;
    }
    /// <summary>
    /// Reference implementation for 3-ary identity values.
    /// </summary>
    /// <typeparam name="Type1">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="Type2">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="Type3">The type of object an instance is primarily an identity value for.</typeparam>
    /// <typeparam name="Key1">The type of the identity component value corresponding to Type1.</typeparam>
    /// <typeparam name="Key2">The type of the identity component value corresponding to Type2.</typeparam>
    /// <typeparam name="Key3">The type of the identity component value corresponding to Type3.</typeparam>
    public class Identity<Type1, Type2, Type3, Key1, Key2, Key3> : IIdentity<Type1, Type2, Type3>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">A reference to the provider of this instance.</param>
        /// <param name="value1">The underlying first component value.</param>
        /// <param name="value2">The underlying second component value.</param>
        /// <param name="value3">The underlying third component value.</param>
        public Identity(IIdentityProvider provider, Key1 value1, Key2 value2, Key3 value3)
          : this(new Identity<Type1, Type2, Key1, Key2>(provider, value1, value2), value3)
        { }
        /// <summary>
        /// Constuctor.
        /// </summary>
        /// <param name="parent">The parent identity.</param>
        /// <param name="value">This identity's component value.</param>
        public Identity(Identity<Type1, Type2, Key1, Key2> parent, Key3 value)
        {
            Provider = parent.Provider;
            ComponentValue = value;
        }

        /// <summary>
        /// Get the identity value's underlying value.
        /// Ternary identity values contain a 3-tuple of component values.
        /// </summary>
        public (Key1, Key2, Key3) Value => (Parent.Parent.ComponentValue, Parent.ComponentValue, ComponentValue);

        /// <summary>
        /// Gets the ternary identity value's third component value.
        /// </summary>
        public Key3 ComponentValue { get; }

        /// <summary>
        /// Gets the identity value's parent.
        /// </summary>
        public Identity<Type1, Type2, Key1, Key2> Parent { get; }

        IIdentity IMultiaryIdentity.Parent => Parent;
        IIdentity<Type1, Type2> IMultiaryIdentity<Type3, IIdentity<Type1,Type2>>.Parent => Parent;

        object IIdentity<Type3>.ComponentValue => ComponentValue;

        /// <summary>
        /// Gets the provider this instance belongs to.
        /// </summary>
        public IIdentityProvider Provider { get; }

        /// <summary>
        /// Gets the type of object the identity value refers to.
        /// </summary>
        public Type ForType => typeof(Type3);

        object IIdentity.Value => Value;

        /// <summary>
        /// Gets the parent chain of identity values. 
        /// </summary>
        public IEnumerable<IIdentity> Identities
        {
            get
            {
                yield return this;
                yield return Parent;
                yield return Parent.Parent;
            }
        }

        /// <summary>
        /// The arity of ternary identity values is always 3.
        /// </summary>
        public int Arity => 3;
    }
}
