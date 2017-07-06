using System.Collections.Generic;

namespace Biz.Morsink.Identity.Utils
{
    /// <summary>
    /// An IEqualityComparer&lt;T&gt; implementation for ValueTuples of arity 2.
    /// </summary>
    /// <typeparam name="A">The first component type.</typeparam>
    /// <typeparam name="B">The second component type.</typeparam>
    public class ValueTupleComparer<A, B> : IEqualityComparer<(A, B)>
    {
        private readonly IEqualityComparer<B> b;
        private readonly IEqualityComparer<A> a;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="a">The equality comparer for the first component.</param>
        /// <param name="b">The equality comparer for the second component.</param>
        public ValueTupleComparer(IEqualityComparer<A> a, IEqualityComparer<B> b)
        {
            this.a = a;
            this.b = b;
        }

        public bool Equals((A, B) x, (A, B) y)
            => a.Equals(x.Item1, y.Item1) && b.Equals(x.Item2, y.Item2);

        public int GetHashCode((A, B) obj)
            => a.GetHashCode(obj.Item1) ^ b.GetHashCode(obj.Item2);
    }
    /// <summary>
    /// An IEqualityComparer&lt;T&gt; implementation for ValueTuples of arity 3.
    /// </summary>
    /// <typeparam name="A">The first component type.</typeparam>
    /// <typeparam name="B">The second component type.</typeparam>
    /// <typeparam name="C">The third component type.</typeparam>
    public class ValueTupleComparer<A, B, C> : IEqualityComparer<(A, B, C)>
    {
        private readonly IEqualityComparer<B> b;
        private readonly IEqualityComparer<A> a;
        private readonly IEqualityComparer<C> c;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="a">The equality comparer for the first component.</param>
        /// <param name="b">The equality comparer for the second component.</param>
        /// <param name="c">The equality comparer for the third component.</param>
        public ValueTupleComparer(IEqualityComparer<A> a, IEqualityComparer<B> b, IEqualityComparer<C> c)
        {
            this.a = a;
            this.b = b;
            this.c = c;
        }

        public bool Equals((A, B, C) x, (A, B, C) y)
            => a.Equals(x.Item1, y.Item1) && b.Equals(x.Item2, y.Item2) && c.Equals(x.Item3, y.Item3);

        public int GetHashCode((A, B, C) obj)
            => a.GetHashCode(obj.Item1) ^ b.GetHashCode(obj.Item2) ^ c.GetHashCode(obj.Item3);
    }
    /// <summary>
    /// An IEqualityComparer&lt;T&gt; implementation for ValueTuples of arity 4.
    /// </summary>
    /// <typeparam name="A">The first component type.</typeparam>
    /// <typeparam name="B">The second component type.</typeparam>
    /// <typeparam name="C">The third component type.</typeparam>
    /// <typeparam name="D">The fourth component type.</typeparam>
    public class ValueTupleComparer<A, B, C, D> : IEqualityComparer<(A, B, C, D)>
    {
        private readonly IEqualityComparer<B> b;
        private readonly IEqualityComparer<A> a;
        private readonly IEqualityComparer<C> c;
        private readonly IEqualityComparer<D> d;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="a">The equality comparer for the first component.</param>
        /// <param name="b">The equality comparer for the second component.</param>
        /// <param name="c">The equality comparer for the third component.</param>
        /// <param name="d">The equality comparer for the fourth component.</param>
        public ValueTupleComparer(IEqualityComparer<A> a, IEqualityComparer<B> b, IEqualityComparer<C> c, IEqualityComparer<D> d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public bool Equals((A, B, C, D) x, (A, B, C, D) y)
            => a.Equals(x.Item1, y.Item1) && b.Equals(x.Item2, y.Item2) && c.Equals(x.Item3, y.Item3) && d.Equals(x.Item4, y.Item4);

        public int GetHashCode((A, B, C, D) obj)
            => a.GetHashCode(obj.Item1) ^ b.GetHashCode(obj.Item2) ^ c.GetHashCode(obj.Item3) ^ d.GetHashCode(obj.Item4);
    }
    /// <summary>
    /// An IEqualityComparer&lt;T&gt; implementation for ValueTuples of arity 5.
    /// </summary>
    /// <typeparam name="A">The first component type.</typeparam>
    /// <typeparam name="B">The second component type.</typeparam>
    /// <typeparam name="C">The third component type.</typeparam>
    /// <typeparam name="D">The fourth component type.</typeparam>
    /// <typeparam name="E">The fifth component type.</typeparam>
    public class ValueTupleComparer<A, B, C, D, E> : IEqualityComparer<(A, B, C, D, E)>
    {
        private readonly IEqualityComparer<B> b;
        private readonly IEqualityComparer<A> a;
        private readonly IEqualityComparer<C> c;
        private readonly IEqualityComparer<D> d;
        private readonly IEqualityComparer<E> e;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="a">The equality comparer for the first component.</param>
        /// <param name="b">The equality comparer for the second component.</param>
        /// <param name="c">The equality comparer for the third component.</param>
        /// <param name="d">The equality comparer for the fourth component.</param>
        /// <param name="e">The equality comparer for the fifth component.</param>
        public ValueTupleComparer(IEqualityComparer<A> a, IEqualityComparer<B> b, IEqualityComparer<C> c, IEqualityComparer<D> d, IEqualityComparer<E> e)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
            this.e = e;
        }

        public bool Equals((A, B, C, D, E) x, (A, B, C, D, E) y)
            => a.Equals(x.Item1, y.Item1) && b.Equals(x.Item2, y.Item2) && c.Equals(x.Item3, y.Item3) && d.Equals(x.Item4, y.Item4) && e.Equals(x.Item5, y.Item5);

        public int GetHashCode((A, B, C, D, E) obj)
            => a.GetHashCode(obj.Item1) ^ b.GetHashCode(obj.Item2) ^ c.GetHashCode(obj.Item3) ^ d.GetHashCode(obj.Item4) ^ e.GetHashCode(obj.Item5);
    }
}
