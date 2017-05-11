using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// A Late Identity class is a placeholder for an identity value that is not known yet. 
    /// Most often, the value will be resolved at some later time by a storage layer.
    /// </summary>
    /// <typeparam name="T">The type the identity value refers to.</typeparam>
    public class LateIdentity<T> : ILateIdentity, IIdentity<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">The provider for this identity value.</param>
        public LateIdentity(IIdentityProvider provider)
        {
            counter = 0;
            Provider = provider;
            value = new NotAvailableValue();
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
    /// <summary>
    /// A Late Identity class is a placeholder for an identity value that is not known yet. 
    /// Most often, the value will be resolved at some later time by a storage layer.
    /// </summary>
    /// <typeparam name="T">The type the identity value refers to.</typeparam>
    /// <typeparam name="K">The type of the actual identity value.</typeparam>
    public class LateIdentity<T, K> : ILateIdentity<K>, IIdentity<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">The provider for this identity value.</param>
        public LateIdentity(IIdentityProvider provider)
        {
            counter = 0;
            Provider = provider;
            notAvailable = new NotAvailableValue();
        }
        private int counter;
        private K value;
        private NotAvailableValue notAvailable;

        public K ComponentValue => value;
        object IIdentity.ComponentValue => IsAvailable ? (object)value : notAvailable;

        public IIdentityProvider Provider { get; }

        public Type ForType => typeof(T);

        public K Value => value;
        object IIdentity.Value => IsAvailable ? (object)value : notAvailable;

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
