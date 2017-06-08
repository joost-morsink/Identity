using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public class SystemIdentity : FreeIdentity<System, string>
    {
        public SystemIdentity(string sys) : base(null, sys) { }
    }
    public class IdentityWithSystem<I> : IIdentity, IIdentityContainer
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

        IIdentity IIdentityContainer.Map(IIdentity id)
            => new IdentityWithSystem<IIdentity>(System, id);
        IIdentity<U> IIdentityContainer.Map<U>(IIdentity<U> id)
            => new IdentityWithSystem<U, IIdentity<U>>(System, id);
    }
    public class IdentityWithSystem<T, I> : IdentityWithSystem<I>, IIdentity<T>
        where I : IIdentity<T>
    {
        public IdentityWithSystem(IIdentity<System> system, I innerIdentity) : base(system, innerIdentity)
        {
        }
    }
    public static class SystemExt
    {
        public static IIdentity WithSystem(this IIdentity id, IIdentity<System> sys)
            => new IdentityWithSystem<IIdentity>(sys, id);
        public static IIdentity<T> WithSystem<T>(this IIdentity<T> id, IIdentity<System> sys)
            => new IdentityWithSystem<T, IIdentity<T>>(sys, id);
    }
    public sealed class System
    {
        private System() { }
    }
}
