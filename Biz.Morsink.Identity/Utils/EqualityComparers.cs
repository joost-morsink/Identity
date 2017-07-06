using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Biz.Morsink.Identity.Utils
{
    /// <summary>
    /// Helper class that acts as a registry for IEqualityComparer&lt;T&gt; instances.
    /// This class is thread-safe.
    /// </summary>
    public class EqualityComparers
    {
        private ConcurrentDictionary<Type, object> comparers;
        /// <summary>
        /// Constructor.
        /// </summary>
        public EqualityComparers()
        {
            comparers = new ConcurrentDictionary<Type, object>();
        }
        /// <summary>
        /// Gets an equality comparer for the specified type.
        /// </summary>
        /// <param name="t">The type of objects that need equality comparing.</param>
        /// <returns>An equality comparer for the specified type.</returns>
        protected object Get(Type t)
            => comparers.GetOrAdd(t, Create);
        /// <summary>
        /// Gets an equality comparer for the specified type.
        /// </summary>
        /// <typeparam name="T">The type of objects that need equality comparing.</typeparam>
        /// <returns>An IEqualityComparer&lt;T&gt; instance.</returns>
        public IEqualityComparer<T> Get<T>()
            => (IEqualityComparer<T>)Get(typeof(T));
        /// <summary>
        /// Sets the IEqualityComparer&lt;T&gt; instance for the specified type.
        /// </summary>
        /// <typeparam name="T">The type of objects that can be compared by the supplied parameter.</typeparam>
        /// <param name="equalityComparer">The equality comparer.</param>
        public void Set<T>(IEqualityComparer<T> equalityComparer)
            => comparers.AddOrUpdate(typeof(T), equalityComparer, (t, o) => equalityComparer);
        /// <summary>
        /// Creates a default equality comparer for the specified type.
        /// Override this method in a derived class to customize equality comparers in a generic way.
        /// </summary>
        /// <param name="t">The type of objects that need equality comparing.</param>
        /// <returns>An equality comparer for the specified type.</returns>
        protected virtual object Create(Type t)
            => typeof(EqualityComparer<>)
                .MakeGenericType(t)
                .GetTypeInfo()
                .GetDeclaredProperty(nameof(EqualityComparer<object>.Default))
                .GetValue(null);
    }
}
