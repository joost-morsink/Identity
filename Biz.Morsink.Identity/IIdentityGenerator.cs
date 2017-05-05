using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity
{
    public interface IIdentityGenerator
    {
        Type ForType { get; }
        IIdentity New(object entity);
    }
    public interface IIdentityGenerator<T> : IIdentityGenerator
    {
        IIdentity<T> New(T entity);
    }
}
