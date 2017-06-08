using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Domain
{
    /// <summary>
    /// This class represents a blog entry.
    /// </summary>
    public class BlogEntry
    {
        /// <summary>
        /// Gets and sets the blog entry's identity value
        /// </summary>
        public IIdentity<Blog, BlogEntry> Id { get; set; }
        /// <summary>
        /// Gets and sets the title.
        /// </summary>
        public string Title { get; set;}
        /// <summary>
        /// Gets and sets the reference to the author.
        /// </summary>
        public IIdentity<User> AuthorId { get; set; }
        /// <summary>
        /// Gets and sets the post date.
        /// </summary>
        public DateTime PostDate { get; set; }
        /// <summary>
        /// Gets and sets the text.
        /// </summary>
        public string Text { get; set; }

    }
}
