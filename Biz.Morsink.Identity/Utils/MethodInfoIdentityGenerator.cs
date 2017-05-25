using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Ex = System.Linq.Expressions.Expression;
namespace Biz.Morsink.Identity.Utils
{
    /// <summary>
    /// Utility class to expose an identity value creating method as an IIdentityGenerator.
    /// </summary>
    /// <typeparam name="T">The type of entity the identity values will refer to.</typeparam>
    public class MethodInfoIdentityGenerator<T> : IIdentityGenerator<T>
    {
        private readonly MethodInfo _method;
        private readonly IIdentityProvider _provider;
        private readonly Func<T, IIdentity<T>> _generator;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="provider">The identity provider the identity values will be associated with.</param>
        /// <param name="methodInfo">The method that actually generates the new identity values.</param>
        public MethodInfoIdentityGenerator(IIdentityProvider provider, MethodInfo methodInfo)
        {
            _method = methodInfo;
            _provider = provider;
            _generator = CreateFunc();
        }
        /// <summary>
        /// Gets the type identity values are created for.
        /// </summary>
        public Type ForType => typeof(T);

        /// <summary>
        /// Gets a new identity value for an entity.
        /// </summary>
        /// <param name="entity">The entity to get a new identity value for.</param>
        /// <returns>A new and unique identity value.</returns>
        public IIdentity<T> New(T entity)
            => _generator(entity);

        /// <summary>
        /// Gets a new identity value for an entity.
        /// </summary>
        /// <param name="entity">The entity to get a new identity value for.</param>
        /// <returns>A new and unique identity value.</returns>
        public IIdentity New(object entity)
            => _generator((T)entity);

        private Func<T, IIdentity<T>> CreateFunc()
        {
            return (Func<T,IIdentity<T>>)_method.CreateDelegate(typeof(Func<T, IIdentity<T>>), _provider);
        }
    }
}
