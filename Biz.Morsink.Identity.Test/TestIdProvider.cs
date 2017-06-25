using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Biz.Morsink.DataConvert;
using System.Linq;
using Biz.Morsink.DataConvert.Converters;

namespace Biz.Morsink.Identity.Test
{
    /// <summary>
    /// An implementation of a ReflectedIdentityProvider for testing purposes.
    /// </summary>
    public class TestIdProvider : ReflectedIdentityProvider
    {
        public static TestIdProvider Instance { get; } = new TestIdProvider();
        private DataConverter _converter = Converters.WithSeparator('-');

        public override IDataConverter GetConverter(Type t, bool incoming)
            => _converter;

        [Creator]
        public Identity<Person, int> PersonId(int value)
            => Builder.For<Person>().Value(value);

        [Creator]
        public Identity<Person, Detail, int, int> DetailId(int p, int d)
            => Builder.For<Person>().Value(p).For<Detail>().Value(d);

        [Creator]
        public Identity<Person, Detail, Sub, int, int, int> SubId(int p, int d, int s)
            => Builder.For<Person>().Value(p).For<Detail>().Value(d).For<Sub>().Value(s);

        [Creator]
        public IIdentity<A> AId(int x)
            => Builder.For<A>().Value(x).Id();
        // This method definition does not satisfy the constraints for identity value creation and will not be used by the generic mechanism.
        [Creator]
        public IIdentity BId(int x)
            => Builder.For<B>().Value(x).Id();

        [Creator]
        public IIdentity<C> CId<K>(K value)
        {
            var b = Builder.For<C>();

            if (typeof(K) == typeof(int) || typeof(K) == typeof(long))
                return b.Value(value).Id();
            else if (GetConverter(typeof(C), true).Convert(value).TryTo(out int v))
                return b.Value(v).Id();
            else
                return null;
        }
        [Creator]
        public IIdentity<D> DId(string value)
            => Builder.For<D>().Value(value).Id();

        private int ids = 0;
        [Generator]
        public IIdentity<A> NewAId(A a = null)
            => AId(Interlocked.Increment(ref ids));
        [Generator]
        public IIdentity<C> NewCId(C c = null)
            => CId(Interlocked.Increment(ref ids));
        [Generator]
        public IIdentity<D> NewDId(D d)
            => DId(d.Code);
    }

}
