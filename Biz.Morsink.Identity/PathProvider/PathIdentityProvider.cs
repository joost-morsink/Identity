using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public class PathIdentityProvider : AbstractIdentityProvider
    {
        public PathIdentityProvider() {

        }

        public override Type GetUnderlyingType(Type forType)
        {
            throw new NotImplementedException();
        }

        protected override IIdentityCreator GetCreator(Type type)
        {
            throw new NotImplementedException();
        }

        protected override IIdentityCreator<T> GetCreator<T>()
        {
            throw new NotImplementedException();
        }
    }
    
}
