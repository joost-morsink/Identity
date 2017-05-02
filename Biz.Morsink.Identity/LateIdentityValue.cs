using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Biz.Morsink.Identity
{
    public class LateIdentityValue<K> : ILateIdentityValue<K>
    {
        public LateIdentityValue()
        {
            counter = 0;
        }
        private int counter;
        private K value;

        public bool IsAvailable => counter > 0;
        public K Value => IsAvailable ? value : throw new InvalidOperationException();

        object ILateIdentityValue.Value => Value;

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

        bool ILateIdentityValue.Resolve(object value)
            => Resolve((K)value);
        public object GetEqualityDelegate()
            => IsAvailable ? (object)Value : null;
    }
    public class LateIdentityValue : ILateIdentityValue
    {
        public LateIdentityValue()
        {
            counter = 0;
        }
        private int counter;
        private object value;

        public bool IsAvailable => counter > 0;
        public object Value => IsAvailable ? value : throw new InvalidOperationException();

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
        public object GetEqualityDelegate()
            => IsAvailable ? Value : null;
    }
}
