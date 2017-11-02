using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// Specifies an object can be identified with an identity value.
    /// </summary>
    public interface IHasIdentity
    {
        /// <summary>
        /// The Identity value of the object.
        /// </summary>
        IIdentity Id { get; }
    }
    /// <summary>
    /// Specifies an object can be identified with an identity value.
    /// </summary>
    /// <typeparam name="T">The entity type of the identity value.</typeparam>
    public interface IHasIdentity<T> : IHasIdentity
    {
        /// <summary>
        /// The Identity value of the object.
        /// </summary>
        new IIdentity<T> Id { get; }
    }
}
