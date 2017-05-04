using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// A late identity interface that supports filling in the actual value in the future.
    /// </summary>
    public interface ILateIdentity : IIdentity
    {
        /// <summary>
        /// Indicates if the identity value is available yet.
        /// </summary>
        bool IsAvailable { get; }
        /// <summary>
        /// Resolve the identity value. This method should be thread safe.
        /// </summary>
        /// <param name="value">The actual identity value.</param>
        /// <returns>True if this call actually resolved the late identity, false otherwise.</returns>
        bool Resolve(object value);
    }
    /// <summary>
    /// A late identity interface that supports filling in the actual typed value in the future.
    /// </summary>
    public interface ILateIdentity<K> : ILateIdentity
    {
        /// <summary>
        /// Resolve the identity value. This method should be thread safe.
        /// </summary>
        /// <param name="value">The actual identity value.</param>
        /// <returns>True if this call actually resolved the late identity, false otherwise.</returns>
        bool Resolve(K value);
    }
}
