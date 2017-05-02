using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public interface ILateIdentity : IIdentity
    {
        bool IsAvailable { get; }
        bool Resolve(object value);
    }
    public interface ILateIdentity<K> : ILateIdentity
    {
        bool Resolve(K value);
    }
}
