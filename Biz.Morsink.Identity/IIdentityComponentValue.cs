using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// This interface describes the underlying component value of the most identifying component (last component).
    /// </summary>
    /// <typeparam name="K">The type of the underlying component value.</typeparam>
    public interface IIdentityComponentValue<K> : IIdentity
    {
        /// <summary>
        /// Gets the underlying component value for this identity value.
        /// </summary>
        new K ComponentValue { get; }
    }
}
