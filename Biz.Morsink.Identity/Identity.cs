using Biz.Morsink.DataConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// Reference implementation for 1-ary indentity values.
    /// </summary>
    /// <typeparam name="Type1">The type of object an instance is an identity value for.</typeparam>
    /// <typeparam name="Key1">The type of the identity value.</typeparam>
    public class Identity<Type1, Key1>
        : IIdentity<Type1>, IIdentityValue<Key1>, IIdentityComponentValue<Key1>
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

        object IIdentity.ComponentValue => Value;

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

        public override int GetHashCode()
            => ForType.GetHashCode() ^ Provider.GetUnderlyingEqualityComparer<Key1>().GetHashCode(Value);
        public override bool Equals(object obj) => Equals(obj as IIdentity<Type1>);
        public bool Equals(IIdentity other) => Equals(other as IIdentity<Type1>);
        public bool Equals(IIdentity<Type1> other)
        {
            if (other == null)
                return false;
            other = Provider.Translate(other);
            Key1 otherVal;
            var typed = other as Identity<Type1, Key1>; // Try same class: cheaper on performance
            if (typed != null)    
                otherVal = typed.Value;
            else
            {
                var ityped = other as IIdentityValue<Key1>; // Try typed interface to avoid conversion
                otherVal = ityped != null ? ityped.Value : Provider.GetConverter(typeof(Type1), true).Convert(other.Value).To<Key1>();
            }
            return Provider.GetUnderlyingEqualityComparer<Key1>().Equals(Value, otherVal);
        }
    }
    /// <summary>
    /// Reference implementation for 2-ary identity values.
    /// </summary>
    /// <typeparam name="Type1">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="Type2">The type of object an instance is primarily an identity value for.</typeparam>
    /// <typeparam name="Key1">The type of the identity component value corresponding to Type1.</typeparam>
    /// <typeparam name="Key2">The type of the identity component value corresponding to Type2.</typeparam>
    public class Identity<Type1, Type2, Key1, Key2>
        : IIdentity<Type1, Type2>, IIdentityValue<(Key1, Key2)>, IIdentityComponentValue<Key2>
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

        object IIdentity.ComponentValue => ComponentValue;

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

        public override int GetHashCode()
            => ForType.GetHashCode() ^ Provider.GetUnderlyingEqualityComparer<(Key1, Key2)>().GetHashCode(Value);
        public override bool Equals(object obj) => Equals(obj as IIdentity<Type2>);
        public bool Equals(IIdentity other) => Equals(other as IIdentity<Type2>);
        public bool Equals(IIdentity<Type2> other)
        {
            if (other == null)
                return false;
            other = Provider.Translate(other);

            (Key1, Key2) otherVal;
            var typed = other as Identity<Type1, Type2, Key1, Key2>; // Try same class: cheaper on performance
            if (typed != null)
                otherVal = typed.Value;
            else
            {
                var ityped = other as IIdentityValue<(Key1, Key2)>; // Try typed interface to avoid conversion
                otherVal = ityped != null ? ityped.Value : Provider.GetConverter(typeof(Type2), true).Convert(other.Value).To<(Key1, Key2)>();
            }
            return Provider.GetUnderlyingEqualityComparer<(Key1, Key2)>().Equals(Value, otherVal);
        }
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
    public class Identity<Type1, Type2, Type3, Key1, Key2, Key3>
        : IIdentity<Type1, Type2, Type3>, IIdentityValue<(Key1, Key2, Key3)>, IIdentityComponentValue<Key3>
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
            Parent = parent;
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
        IIdentity<Type1, Type2> IMultiaryIdentity<Type3, IIdentity<Type1, Type2>>.Parent => Parent;

        object IIdentity.ComponentValue => ComponentValue;

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

        public override int GetHashCode()
            => ForType.GetHashCode() ^ Provider.GetUnderlyingEqualityComparer<(Key1, Key2, Key3)>().GetHashCode(Value);
        public override bool Equals(object obj) => Equals(obj as IIdentity<Type3>);
        public bool Equals(IIdentity other) => Equals(other as IIdentity<Type3>);

        public bool Equals(IIdentity<Type3> other)
        {
            if (other == null)
                return false;
            other = Provider.Translate(other);
            (Key1, Key2, Key3) otherVal;
            var typed = other as Identity<Type1, Type2, Type3, Key1, Key2, Key3>; // Try same class: cheaper on performance
            if (typed != null)
                otherVal = typed.Value;
            else
            {
                var ityped = other as IIdentityValue<(Key1, Key2, Key3)>; // Try typed interface to avoid conversion
                otherVal = ityped != null ? ityped.Value : Provider.GetConverter(typeof(Type3), true).Convert(other.Value).To<(Key1, Key2, Key3)>();
            }
            return Provider.GetUnderlyingEqualityComparer<(Key1, Key2, Key3)>().Equals(Value, otherVal);
        }
    }

    /// <summary>
    /// Reference implementation for 4-ary identity values.
    /// </summary>
    /// <typeparam name="Type1">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="Type2">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="Type3">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="Type4">The type of object an instance is primarily an identity value for.</typeparam>
    /// <typeparam name="Key1">The type of the identity component value corresponding to Type1.</typeparam>
    /// <typeparam name="Key2">The type of the identity component value corresponding to Type2.</typeparam>
    /// <typeparam name="Key3">The type of the identity component value corresponding to Type3.</typeparam>
    /// <typeparam name="Key4">The type of the identity component value corresponding to Type4.</typeparam>
    public class Identity<Type1, Type2, Type3, Type4, Key1, Key2, Key3, Key4>
        : IIdentity<Type1, Type2, Type3, Type4>, IIdentityValue<(Key1, Key2, Key3, Key4)>, IIdentityComponentValue<Key4>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">A reference to the provider of this instance.</param>
        /// <param name="value1">The underlying first component value.</param>
        /// <param name="value2">The underlying second component value.</param>
        /// <param name="value3">The underlying third component value.</param>
        /// <param name="value4">The underlying fourth component value.</param>
        public Identity(IIdentityProvider provider, Key1 value1, Key2 value2, Key3 value3, Key4 value4)
          : this(new Identity<Type1, Type2, Type3, Key1, Key2, Key3>(provider, value1, value2, value3), value4)
        { }
        /// <summary>
        /// Constuctor.
        /// </summary>
        /// <param name="parent">The parent identity.</param>
        /// <param name="value">This identity's component value.</param>
        public Identity(Identity<Type1, Type2, Type3, Key1, Key2, Key3> parent, Key4 value)
        {
            Provider = parent.Provider;
            Parent = parent;
            ComponentValue = value;
        }

        /// <summary>
        /// Get the identity value's underlying value.
        /// Ternary identity values contain a 4-tuple of component values.
        /// </summary>
        public (Key1, Key2, Key3, Key4) Value => (Parent.Parent.Parent.ComponentValue, Parent.Parent.ComponentValue, Parent.ComponentValue, ComponentValue);

        /// <summary>
        /// Gets the 4-ary identity value's fourth component value.
        /// </summary>
        public Key4 ComponentValue { get; }

        /// <summary>
        /// Gets the identity value's parent.
        /// </summary>
        public Identity<Type1, Type2, Type3, Key1, Key2, Key3> Parent { get; }

        IIdentity IMultiaryIdentity.Parent => Parent;
        IIdentity<Type1, Type2, Type3> IMultiaryIdentity<Type4, IIdentity<Type1, Type2, Type3>>.Parent => Parent;

        object IIdentity.ComponentValue => ComponentValue;

        /// <summary>
        /// Gets the provider this instance belongs to.
        /// </summary>
        public IIdentityProvider Provider { get; }

        /// <summary>
        /// Gets the type of object the identity value refers to.
        /// </summary>
        public Type ForType => typeof(Type4);

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
                yield return Parent.Parent.Parent;
            }
        }

        /// <summary>
        /// The arity of 4-ary identity values is always 4.
        /// </summary>
        public int Arity => 4;

        public override int GetHashCode()
            => ForType.GetHashCode() ^ Provider.GetUnderlyingEqualityComparer<(Key1, Key2, Key3, Key4)>().GetHashCode(Value);
        public override bool Equals(object obj) => Equals(obj as IIdentity<Type4>);
        public bool Equals(IIdentity other) => Equals(other as IIdentity<Type4>);

        public bool Equals(IIdentity<Type4> other)
        {
            if (other == null)
                return false;
            other = Provider.Translate(other);
            (Key1, Key2, Key3, Key4) otherVal;
            var typed = other as Identity<Type1, Type2, Type3, Type4, Key1, Key2, Key3, Key4>; // Try same class: cheaper on performance
            if (typed != null)
                otherVal = typed.Value;
            else
            {
                var ityped = other as IIdentityValue<(Key1, Key2, Key3, Key4)>; // Try typed interface to avoid conversion
                otherVal = ityped != null ? ityped.Value : Provider.GetConverter(typeof(Type4), true).Convert(other.Value).To<(Key1, Key2, Key3, Key4)>();
            }
            return Provider.GetUnderlyingEqualityComparer<(Key1, Key2, Key3, Key4)>().Equals(Value, otherVal);
        }
    }
    /// <summary>
    /// Reference implementation for 5-ary identity values.
    /// </summary>
    /// <typeparam name="Type1">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="Type2">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="Type3">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="Type4">A type for which an instance is an identity value for in the parent chain.</typeparam>
    /// <typeparam name="Type5">The type of object an instance is primarily an identity value for.</typeparam>
    /// <typeparam name="Key1">The type of the identity component value corresponding to Type1.</typeparam>
    /// <typeparam name="Key2">The type of the identity component value corresponding to Type2.</typeparam>
    /// <typeparam name="Key3">The type of the identity component value corresponding to Type3.</typeparam>
    /// <typeparam name="Key4">The type of the identity component value corresponding to Type4.</typeparam>
    /// <typeparam name="Key5">The type of the identity component value corresponding to Type5.</typeparam>
    public class Identity<Type1, Type2, Type3, Type4, Type5, Key1, Key2, Key3, Key4, Key5>
        : IIdentity<Type1, Type2, Type3, Type4, Type5>, IIdentityValue<(Key1, Key2, Key3, Key4, Key5)>, IIdentityComponentValue<Key5>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">A reference to the provider of this instance.</param>
        /// <param name="value1">The underlying first component value.</param>
        /// <param name="value2">The underlying second component value.</param>
        /// <param name="value3">The underlying third component value.</param>
        /// <param name="value4">The underlying fourth component value.</param>
        public Identity(IIdentityProvider provider, Key1 value1, Key2 value2, Key3 value3, Key4 value4, Key5 value5)
          : this(new Identity<Type1, Type2, Type3, Type4, Key1, Key2, Key3, Key4>(provider, value1, value2, value3, value4), value5)
        { }
        /// <summary>
        /// Constuctor.
        /// </summary>
        /// <param name="parent">The parent identity.</param>
        /// <param name="value">This identity's component value.</param>
        public Identity(Identity<Type1, Type2, Type3, Type4, Key1, Key2, Key3, Key4> parent, Key5 value)
        {
            Provider = parent.Provider;
            Parent = parent;
            ComponentValue = value;
        }

        /// <summary>
        /// Get the identity value's underlying value.
        /// Ternary identity values contain a 5-tuple of component values.
        /// </summary>
        public (Key1, Key2, Key3, Key4, Key5) Value => (Parent.Parent.Parent.Parent.ComponentValue, Parent.Parent.Parent.ComponentValue, Parent.Parent.ComponentValue, Parent.ComponentValue, ComponentValue);

        /// <summary>
        /// Gets the 4-ary identity value's fourth component value.
        /// </summary>
        public Key5 ComponentValue { get; }

        /// <summary>
        /// Gets the identity value's parent.
        /// </summary>
        public Identity<Type1, Type2, Type3, Type4, Key1, Key2, Key3, Key4> Parent { get; }

        IIdentity IMultiaryIdentity.Parent => Parent;
        IIdentity<Type1, Type2, Type3, Type4> IMultiaryIdentity<Type5, IIdentity<Type1, Type2, Type3, Type4>>.Parent => Parent;

        object IIdentity.ComponentValue => ComponentValue;

        /// <summary>
        /// Gets the provider this instance belongs to.
        /// </summary>
        public IIdentityProvider Provider { get; }

        /// <summary>
        /// Gets the type of object the identity value refers to.
        /// </summary>
        public Type ForType => typeof(Type5);

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
                yield return Parent.Parent.Parent;
            }
        }

        /// <summary>
        /// The arity of 5-ary identity values is always 5.
        /// </summary>
        public int Arity => 5;

        public override int GetHashCode()
            => ForType.GetHashCode() ^ Provider.GetUnderlyingEqualityComparer<(Key1, Key2, Key3, Key4, Key5)>().GetHashCode(Value);
        public override bool Equals(object obj) => Equals(obj as IIdentity<Type5>);
        public bool Equals(IIdentity other) => Equals(other as IIdentity<Type5>);

        public bool Equals(IIdentity<Type5> other)
        {
            if (other == null)
                return false;
            other = Provider.Translate(other);
            (Key1, Key2, Key3, Key4, Key5) otherVal;
            var typed = other as Identity<Type1, Type2, Type3, Type4, Type5, Key1, Key2, Key3, Key4, Key5>; // Try same class: cheaper on performance
            if (typed != null)
                otherVal = typed.Value;
            else
            {
                var ityped = other as IIdentityValue<(Key1, Key2, Key3, Key4, Key5)>; // Try typed interface to avoid conversion
                otherVal = ityped != null ? ityped.Value : Provider.GetConverter(typeof(Type5), true).Convert(other.Value).To<(Key1, Key2, Key3, Key4, Key5)>();
            }
            return Provider.GetUnderlyingEqualityComparer<(Key1, Key2, Key3, Key4, Key5)>().Equals(Value, otherVal);
        }
    }
}
