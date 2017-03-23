using Biz.Morsink.DataConvert;
using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public interface IIdentityProvider
    {
        IIdentity Translate(IIdentity id);
        IDataConverter Converter { get; }
    }
}
