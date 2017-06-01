using Biz.Morsink.Identity.PathProvider;
using Biz.Morsink.Identity.Test.WebApplication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.IdentityProvider
{
    public class ApiIdentityProvider : PathIdentityProvider
    {
        public static ApiIdentityProvider Instance { get; } = new ApiIdentityProvider();
        public ApiIdentityProvider()
        {
            AddEntry("/user/*", typeof(User));
            AddEntry("/blog/*", typeof(Blog));
            AddEntry("/blog/*/*", typeof(Blog), typeof(BlogEntry));
            AddEntry("/blog/*/*/comments/*", typeof(Blog), typeof(BlogEntry), typeof(Comment));
        }
    }
}
