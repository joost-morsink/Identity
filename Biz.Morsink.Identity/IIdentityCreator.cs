using System;

namespace Biz.Morsink.Identity
{
    public interface IIdentityCreator
    {
        IIdentity Create<K>(K value);
    }
    public interface IIdentityCreator<T> : IIdentityCreator
    {
        new IIdentity<T> Create<K>(K value);
    }
}
