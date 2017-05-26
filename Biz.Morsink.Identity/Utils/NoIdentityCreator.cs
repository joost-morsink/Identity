using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.Utils
{
    public class NoIdentityCreator<T> : IIdentityCreator<T>
    {
        public IIdentity<T> Create<K>(K value)
            => null;

        IIdentity IIdentityCreator.Create<K>(K value)
            => null;
    }
}
