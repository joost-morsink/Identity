using Biz.Morsink.Identity.Test.WebApplication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Repositories
{
    public class BackendIdentityProvider : ReflectedIdentityProvider
    {
        public static BackendIdentityProvider Instance { get; } = new BackendIdentityProvider();
        [Creator]
        public IIdentity<User> UserId(string name)
            => new Identity<User, string>(this, name);
        [Creator]
        public IIdentity<Blog> BlogId(string name)
            => new Identity<Blog, string>(this, name);
        [Creator]
        public IIdentity<Blog, BlogEntry> BlogEntryId(string blog, string title)
            => new Identity<Blog, BlogEntry, string, string>(this, blog, title);
        [Creator]
        public IIdentity<Blog, BlogEntry, Comment> CommentId(string blog, string title, int order)
            => new Identity<Blog, BlogEntry, Comment, string, string, int>(this, blog, title, order);

        //public override bool SupportsNewIdentities => true;

        //[Generator]
        //public IIdentity<User> GenUserId(User user)
        //    => UserId(user.Name);
        //[Generator]
        //public IIdentity<Blog> GenBlogId(Blog blog)
        //    => 
    }
}
