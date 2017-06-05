using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Domain
{
    /// <summary>
    /// This class represents a comment on a blog entry.
    /// </summary>
    public class Comment
    {
        /// <summary>
        /// Gets and sets the comment's identity value.
        /// </summary>
        public IIdentity<Blog, BlogEntry, Comment> Id { get; set; }
        /// <summary>
        /// Get and sets the order number of the comment.
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// Gets and sets the reference to the user making the comment.
        /// </summary>
        public IIdentity<User> UserId { get; set; }
        /// <summary>
        /// Gets and sets the comment timestamp.
        /// </summary>
        public DateTime CommentDate { get; set; }
        /// <summary>
        /// Gets and sets the text.
        /// </summary>
        public string Text { get; set; }
    }
}
