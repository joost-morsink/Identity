using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.Systems
{
    /// <summary>
    /// This class represents an identity value classified within a system.
    /// </summary>
    /// <typeparam name="I">The type of identity value.</typeparam>
    public class IdentityWithSystem<I> : IIdentity, IIdentityContainer
        where I : IIdentity
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="system">The system identity value.</param>
        /// <param name="innerIdentity">The identity value.</param>
        public IdentityWithSystem(IIdentity<Sys> system, I innerIdentity)
        {
            System = system;
            InnerIdentity = innerIdentity;
        }
        /// <summary>
        /// Gets the system identity value.
        /// </summary>
        public IIdentity<Sys> System { get; }
        /// <summary>
        /// Gets the inner identity value.
        /// </summary>
        public I InnerIdentity { get; }

        /// <summary>
        /// The Provider is the InnerIdentity's Provider.
        /// </summary>
        public IIdentityProvider Provider => InnerIdentity.Provider;

        /// <summary>
        /// The ForType is the InnerIdentity's ForType.
        /// </summary>
        public Type ForType => InnerIdentity.ForType;

        /// <summary>
        /// The Value is the InnerIdentity's Value.
        /// </summary>
        public object Value => InnerIdentity.Value;

        /// <summary>
        /// The ComponentValue is the Identity's ComponentValue.
        /// </summary>
        public object ComponentValue => InnerIdentity.ComponentValue;

        /// <summary>
        /// The Identities are the InnerIdentity's Identities and the system identity value.
        /// </summary>
        public IEnumerable<IIdentity> Identities
        {
            get
            {
                foreach (var id in InnerIdentity.Identities)
                    yield return id;
                yield return System;
            }
        }
        /// <summary>
        /// The Arity is the Identity's Arity.
        /// </summary>
        public int Arity => InnerIdentity.Arity;

        IIdentity IIdentityContainer.Map(IIdentity id)
            => new IdentityWithSystem<IIdentity>(System, id);
        IIdentity<U> IIdentityContainer.Map<U>(IIdentity<U> id)
            => new IdentityWithSystem<U, IIdentity<U>>(System, id);
    }
    /// <summary>
    /// This class represents an identity value classified within a system.
    /// </summary>
    /// <typeparam name="T">The entity type of the inner identity value.</typeparam>
    /// <typeparam name="I">The type of identity value.</typeparam>
    public class IdentityWithSystem<T, I> : IdentityWithSystem<I>, IIdentity<T>
        where I : IIdentity<T>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="system">The system identity value.</param>
        /// <param name="innerIdentity">The identity value.</param>
        public IdentityWithSystem(IIdentity<Sys> system, I innerIdentity) : base(system, innerIdentity)
        {
        }
    }

}
