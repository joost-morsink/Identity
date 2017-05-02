using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public interface ILateIdentityValue : IDelegateEquality
    {
        bool IsAvailable { get; }
        object Value { get; }
        bool Resolve(object value);
    }
    public interface ILateIdentityValue<K> : ILateIdentityValue
    {
        new K Value { get; }
        bool Resolve(K value);
    }
}
