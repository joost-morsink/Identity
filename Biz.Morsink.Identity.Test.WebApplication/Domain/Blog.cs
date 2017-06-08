using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Domain
{
    /// <summary>
    /// This class represents a blog.
    /// </summary>
    public class Blog
    {
        /// <summary>
        /// Gets and sets the blog's identity value.
        /// </summary>
        public IIdentity<Blog> Id { get; set; }
        /// <summary>
        /// Gets and sets the name.
        /// The value should be short and identifier-like.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets and sets the title.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets and set the blog's user.
        /// </summary>
        public IIdentity<User> Owner { get; set; }
    }
}
