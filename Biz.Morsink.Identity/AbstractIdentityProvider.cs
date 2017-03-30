using System;
using System.Collections.Generic;
using System.Text;
using Biz.Morsink.DataConvert;

namespace Biz.Morsink.Identity
{
    public class AbstractIdentityProvider : IIdentityProvider
    {
        public virtual bool Equals(IIdentity x, IIdentity y)
            => object.ReferenceEquals(x, y)
            || x.ForType == y.ForType
                && object.Equals(x.Value, GetConverter(x.ForType, true).GetGeneralConverter(y.Value.GetType(), x.Value.GetType())(y.Value).Result);
        
        public virtual IDataConverter GetConverter(Type t, bool incoming)
            => DataConverter.Default;

        public int GetHashCode(IIdentity obj)
            => obj.ForType.GetHashCode() ^ (obj.Value?.GetHashCode() ?? 0);

        public virtual IIdentity Translate(IIdentity id)
        {
            throw new NotImplementedException();
        }
    }
}
