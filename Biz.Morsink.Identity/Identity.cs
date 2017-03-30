using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public class Identity<Type1, Key1> : IIdentity<Type1>
    {
        public Identity(IIdentityProvider provider, Key1 value)
        {
            Provider = provider;
            Value = value;
        }
        public Key1 Value { get; }
        public Key1 ComponentValue => Value;

        object IIdentity<Type1>.ComponentValue => Value;

        public IIdentityProvider Provider { get; }

        public Type ForType => typeof(Type1);

        object IIdentity.Value => Value;

        public IEnumerable<IIdentity> Identities
        {
            get { yield return this; }
        }
        public int Arity => 1;
    }
    public class Identity<Type1, Type2, Key1, Key2> : IIdentity<Type1, Type2>
    {
        public Identity(IIdentityProvider provider, Key1 value1, Key2 value2)
            : this(new Identity<Type1, Key1>(provider, value1), value2)
        { }
        public Identity(Identity<Type1, Key1> parent, Key2 value)
        {
            Provider = parent.Provider;
            ComponentValue = value;
        }

        public (Key1, Key2) Value => (Parent.ComponentValue, ComponentValue);

        public Key2 ComponentValue { get; }

        public Identity<Type1, Key1> Parent { get; }

        IIdentity<Type1> IMultiaryIdentity<Type2, IIdentity<Type1>>.Parent => Parent;

        object IIdentity<Type2>.ComponentValue => ComponentValue;

        public IIdentityProvider Provider { get; }

        public Type ForType => typeof(Type2);

        object IIdentity.Value => Value;

        public IEnumerable<IIdentity> Identities
        {
            get
            {
                yield return this;
                yield return Parent;
            }
        }

        public int Arity => 2;
    }
    public class Identity<Type1, Type2, Type3, Key1, Key2, Key3> : IIdentity<Type1, Type2, Type3>
    {
        public Identity(IIdentityProvider provider, Key1 value1, Key2 value2, Key3 value3)
          : this(new Identity<Type1, Type2, Key1, Key2>(provider, value1, value2), value3)
        { }
        public Identity(Identity<Type1, Type2, Key1, Key2> parent, Key3 value)
        {
            Provider = parent.Provider;
            ComponentValue = value;
        }

        public (Key1, Key2, Key3) Value => (Parent.Parent.ComponentValue, Parent.ComponentValue, ComponentValue);

        public Key3 ComponentValue { get; }

        public Identity<Type1, Type2, Key1, Key2> Parent { get; }

        IIdentity<Type1, Type2> IMultiaryIdentity<Type3, IIdentity<Type1,Type2>>.Parent => Parent;

        object IIdentity<Type3>.ComponentValue => ComponentValue;

        public IIdentityProvider Provider { get; }

        public Type ForType => typeof(Type3);

        object IIdentity.Value => Value;

        public IEnumerable<IIdentity> Identities
        {
            get
            {
                yield return this;
                yield return Parent;
                yield return Parent.Parent;
            }
        }

        public int Arity => 3;
    }
}
