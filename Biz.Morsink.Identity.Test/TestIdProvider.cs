using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Biz.Morsink.DataConvert;

namespace Biz.Morsink.Identity.Test
{
    public class TestIdProvider : ReflectedIdentityProvider
    {
        public static TestIdProvider Instance { get; } = new TestIdProvider();

        public Identity<Person, int> PersonId(int value)
            => new Identity<Person, int>(this, value);

        public Identity<Person, Detail, int, int> DetailId(int p, int d)
            => new Identity<Person, Detail, int, int>(this, p, d);

        public IIdentity<A> AId(int x)
            => new Identity<A, int>(this, x);

        // This method definition does not satisfy the constraints for identity value creation and will not be used by the generic mechanism.
        public IIdentity BId(int x)
            => new Identity<B, int>(this, x);

        public IIdentity<C> CId<K>(K value)
        {
            if (typeof(K) == typeof(int) || typeof(K) == typeof(long))
                return new Identity<C, K>(this, value);
            else if (GetConverter(typeof(C), true).Convert(value).TryTo(out int v))
                return new Identity<C, int>(this, v);
            else
                return null;
        }
        public IIdentity<D> DId(string value)
            => new Identity<D, string>(this, value);

        private int ids = 0;
        public IIdentity<A> NewAId(A a)
            => AId(Interlocked.Increment(ref ids));
        public IIdentity<C> NewCId(C c)
            => CId(Interlocked.Increment(ref ids));
        public IIdentity<D> NewDId(D d)
            => DId(d.Code);
    }

}
