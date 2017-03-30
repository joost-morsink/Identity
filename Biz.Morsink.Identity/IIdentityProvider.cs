using Biz.Morsink.DataConvert;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public interface IIdentityProvider : IEqualityComparer<IIdentity>
    {
        IIdentity Translate(IIdentity id);
        IDataConverter GetConverter(Type t, bool incoming);
    }
}
