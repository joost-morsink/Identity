using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// Identity provider implementation for 'free' identities.
    /// </summary>
    public class FreeIdentityProvider : AbstractIdentityProvider
    {
        /// <summary>
        /// Static instance of the FreeIdentityProvider.
        /// </summary>
        public static FreeIdentityProvider Instance { get; } = new FreeIdentityProvider();
        private class Creator<T> : IIdentityCreator<T>
        {
            public static Creator<T> Instance { get; } = new Creator<T>();
            public IIdentity<T> Create<K>(K value)
                => FreeIdentity<T>.Create(value);

            IIdentity IIdentityCreator.Create<K>(K value)
                => Create(value);
        }
        /// <summary>
        /// The underlying type for all free identities is free
        /// </summary>
        /// <param name="forType">The entity type.</param>
        /// <returns>typeof(object)</returns>
        public override Type GetUnderlyingType(Type forType)
            => typeof(object);

        protected override IIdentityCreator GetCreator(Type type)
            => (IIdentityCreator)Activator.CreateInstance(typeof(Creator<>).MakeGenericType(type));

        protected override IIdentityCreator<T> GetCreator<T>()
            => Creator<T>.Instance;
        
    }
}
