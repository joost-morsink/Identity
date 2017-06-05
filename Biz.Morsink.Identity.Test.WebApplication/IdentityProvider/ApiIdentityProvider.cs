using Biz.Morsink.Identity.PathProvider;
using Biz.Morsink.Identity.Test.WebApplication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.IdentityProvider
{
    /// <summary>
    /// The ApiIdentityProvider extens the PathIdentityProvider.
    /// It contains path mappings for User, Blog, BlogEntry and Comment types.
    /// </summary>
    public class ApiIdentityProvider : PathIdentityProvider
    {
        /// <summary>
        /// A singleton instance.
        /// </summary>
        public static ApiIdentityProvider Instance { get; } = new ApiIdentityProvider();
        /// <summary>
        /// Constructor.
        /// </summary>
        public ApiIdentityProvider()
        {
            AddEntry("/user/*", typeof(User));
            AddEntry("/blog/*", typeof(Blog));
            AddEntry("/blog/*/*", typeof(Blog), typeof(BlogEntry));
            AddEntry("/blog/*/*/comments/*", typeof(Blog), typeof(BlogEntry), typeof(Comment));
        }
    }
}
