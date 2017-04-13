using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// This interface describes identity values of arity > 1.
    /// </summary>
    /// <typeparam name="T">The type of object an instance is primarily an identity value for.</typeparam>
    /// <typeparam name="P">The type of the parent's identity value.</typeparam>
    public interface IMultiaryIdentity<T, P> : IIdentity<T>
        where P: IIdentity
    {
        /// <summary>
        /// Gets the parent's identity value.
        /// </summary>
        P Parent { get; }
    }
}
