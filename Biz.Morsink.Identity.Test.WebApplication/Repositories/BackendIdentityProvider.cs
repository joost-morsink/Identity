using Biz.Morsink.Identity.Test.WebApplication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Repositories
{
    /// <summary>
    /// This component represents 'some' backend storage layer for the domain objects in this example.
    /// </summary>
    public class BackendIdentityProvider : ReflectedIdentityProvider
    {
        /// <summary>
        /// A singleton instance.
        /// </summary>
        public static BackendIdentityProvider Instance { get; } = new BackendIdentityProvider();
        /// <summary>
        /// Creates a User identity value.
        /// </summary>
        /// <param name="name">The username.</param>
        /// <returns>A User identity value.</returns>
        [Creator]
        public IIdentity<User> UserId(string name)
            => new Identity<User, string>(this, name);
        /// <summary>
        /// Creates a Blog identity value.
        /// </summary>
        /// <param name="name">The blog name.</param>
        /// <returns>A Blog identity value.</returns>
        [Creator]
        public IIdentity<Blog> BlogId(string name)
            => new Identity<Blog, string>(this, name);
        /// <summary>
        /// Creates a BlogEntry identity value.
        /// </summary>
        /// <param name="blog">The blog name.</param>
        /// <param name="title">The entry title.</param>
        /// <returns>A BlogEntry identity value.</returns>
        [Creator]
        public IIdentity<Blog, BlogEntry> BlogEntryId(string blog, string title)
            => new Identity<Blog, BlogEntry, string, string>(this, blog, title);
        /// <summary>
        /// Creates a Comment identity value.
        /// </summary>
        /// <param name="blog">The blog name.</param>
        /// <param name="title">Then entry title.</param>
        /// <param name="order">The order of the comment within the entry.</param>
        /// <returns><A Comment identity value.</returns>
        [Creator]
        public IIdentity<Blog, BlogEntry, Comment> CommentId(string blog, string title, int order)
            => new Identity<Blog, BlogEntry, Comment, string, string, int>(this, blog, title, order);
    }
}
