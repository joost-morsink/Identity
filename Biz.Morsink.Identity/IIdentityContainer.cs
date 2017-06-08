using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public interface IIdentityContainer : IIdentity
    {
        IIdentity Map(IIdentity id);
        IIdentity<T> Map<T>(IIdentity<T> id);
    }
    static class IdentityContainerExt
    {
        public static IIdentity MapContainer(this IIdentity container, IIdentity id)
        {
            var c = container as IIdentityContainer;
            return c == null ? id : c.Map(id);
        }
        public static IIdentity<T> MapContainer<T>(this IIdentity container, IIdentity<T> id)
        {
            var c = container as IIdentityContainer;
            return c == null ? id : c.Map(id);
        }
    }
}
