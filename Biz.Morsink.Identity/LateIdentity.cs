using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Biz.Morsink.Identity
{
    public class LateIdentity<T> : ILateIdentity, IIdentity<T>
    {
        public LateIdentity(IIdentityProvider provider)
        {
            counter = 0;
            Provider = provider;
        }
        private int counter;
        private object value;
        public object ComponentValue => value;

        public IIdentityProvider Provider { get; }

        public Type ForType => typeof(T);

        public object Value => value;

        public IEnumerable<IIdentity> Identities
        {
            get
            {
                yield return this;
            }
        }

        public int Arity => 1;

        public bool IsAvailable => counter > 0;

        public bool Resolve(object value)
        {
            if (Interlocked.Increment(ref counter) == 1)
            {
                this.value = value;
                return true;
            }
            else
                return false;
        }
    }
    public class LateIdentity<T, K> : ILateIdentity<K>, IIdentity<T>
    {
        public LateIdentity(IIdentityProvider provider)
        {
            counter = 0;
            Provider = provider;
        }
        private int counter;
        private K value;

        public K ComponentValue => value;
        object IIdentity<T>.ComponentValue => IsAvailable ? (object)value : null;

        public IIdentityProvider Provider { get; }

        public Type ForType => typeof(T);

        public K Value => value;
        object IIdentity.Value => IsAvailable ? (object)value : null;

        public IEnumerable<IIdentity> Identities
        {
            get
            {
                yield return this;
            }
        }

        public int Arity => 1;

        public bool IsAvailable => counter > 0;

        public bool Resolve(K value)
        {
            if (Interlocked.Increment(ref counter) == 1)
            {
                this.value = value;
                return true;
            }
            else
                return false;
        }

        bool ILateIdentity.Resolve(object value)
            => Resolve((K)value);
    }
}
