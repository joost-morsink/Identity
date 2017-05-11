using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    /// <summary>
    /// This interface describes identity values of arity > 1, without either specifying what type the current identity refers to or what the parent refers to.
    /// </summary>
    public interface IMultiaryIdentity
    {
        IIdentity Parent { get; }
    }
    /// <summary>
    /// This interface describes idenity values of arity > 1, without specifying exactly what the parent type is.
    /// </summary>
    /// <typeparam name="T">The type of object an instance is primarily an identity value for.</typeparam>
    public interface IMultiaryIdentity<T> : IMultiaryIdentity, IIdentity<T>
    {
    }
    /// <summary>
    /// This interface describes identity values of arity > 1.
    /// </summary>
    /// <typeparam name="T">The type of object an instance is primarily an identity value for.</typeparam>
    /// <typeparam name="P">The type of the parent's identity value.</typeparam>
    public interface IMultiaryIdentity<T, P> : IMultiaryIdentity<T>
        where P : IIdentity
    {
        /// <summary>
        /// Gets the parent's identity value.
        /// </summary>
        new P Parent { get; }
    }
}
