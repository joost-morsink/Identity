using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public interface IMultiaryIdentity<T, P> : IIdentity<T>
        where P: IIdentity
    {
        P Parent { get; }
    }
}
