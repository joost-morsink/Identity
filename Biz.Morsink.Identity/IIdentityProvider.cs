using Biz.Morsink.DataConvert;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public interface IIdentityProvider : IEqualityComparer<IIdentity>
    {
        IIdentity Translate(IIdentity id);
        IIdentity<T> Translate<T>(IIdentity<T> id);
        IDataConverter GetConverter(Type t, bool incoming);
        IIdentity Create<K>(Type forType, K value);
        IIdentity<T> Create<T, K>(K value);
    }
}
