using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// This interface describes the underlying value for an identity value.
    /// </summary>
    /// <typeparam name="K">The type of the underlying value.</typeparam>
    public interface IIdentityValue<K> : IIdentity
    {
        /// <summary>
        /// Gets the underlying value for this identity value.
        /// </summary>
        new K Value { get; }
    }
}
