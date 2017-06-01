using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.Utils
{
    /// <summary>
    /// An identity creator that does not create any identity values.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public class NoIdentityCreator<T> : IIdentityCreator<T>
    {
        /// <summary>
        /// Does not create an identity value, no matter what the parameter.
        /// </summary>
        /// <returns>null</returns>
        public IIdentity<T> Create<K>(K value)
            => null;

        IIdentity IIdentityCreator.Create<K>(K value)
            => null;
    }
}
