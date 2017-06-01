using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Generic
{
    public interface IRead<T>
        where T : class
    {
        T Read(IIdentity<T> id);
    }
}
