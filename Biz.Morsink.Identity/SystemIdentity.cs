using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public class SystemIdentity : FreeIdentity<System, string>
    {
        public SystemIdentity(string sys) : base(null, sys) { }
    }
    public class IdentityWithSystem<I> : IIdentity
        where I : IIdentity
    {
        public IdentityWithSystem(IIdentity<System> system, I innerIdentity)
        {
            System = system;
            InnerIdentity = innerIdentity;
        }
        public IIdentity<System> System { get; }
        public I InnerIdentity { get; }

        public IIdentityProvider Provider => InnerIdentity.Provider;

        public Type ForType => InnerIdentity.ForType;

        public object Value => InnerIdentity.Value;

        public object ComponentValue => InnerIdentity.ComponentValue;

        public IEnumerable<IIdentity> Identities
        {
            get
            {
                foreach (var id in InnerIdentity.Identities)
                    yield return id;
                yield return System;
            }
        }

        public int Arity => InnerIdentity.Arity;
    }
    public class IdentityWithSystem<T, I> : IdentityWithSystem<I>, IIdentity<T>
        where I : IIdentity<T>
    {
        public IdentityWithSystem(IIdentity<System> system, I innerIdentity) : base(system, innerIdentity)
        {
        }
    }
    public sealed class System
    {
        private System() { }
    }
}
