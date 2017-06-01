using Biz.Morsink.Identity.Test.WebApplication.Domain;
using Biz.Morsink.Identity.Test.WebApplication.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Repositories
{
    public class BlogRepository : IRead<Blog>, IRead<BlogEntry>, IRead<Comment>
    {
        private static BackendIdentityProvider IdProvider = BackendIdentityProvider.Instance;

        private List<Blog> blogs = new List<Blog>
        {
            new Blog
            {
                Id = IdProvider.BlogId("Tech"),
                Name = "Tech",
                Title = "Joost's technology blog"
            }
        };
        private List<BlogEntry> entries = new List<BlogEntry>
        {
            new BlogEntry
            {
                Id = IdProvider.BlogEntryId("Tech", "Lorem"),
                AuthorId = IdProvider.UserId("Joost"),
                PostDate = new DateTime(2017, 6, 1, 13, 3, 0,DateTimeKind.Utc),
                Title = "Lorem ipsum dolor sit amet",
                Text = @"Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.
Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. 
Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. 
Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum."
            }
        };
        private List<Comment> comments = new List<Comment> {
            new Comment
            {
                Id = IdProvider.CommentId("Tech", "Lorem", 1),
                UserId = IdProvider.UserId("guest"),
                CommentDate = new DateTime(2017, 6, 1, 13, 7, 0, DateTimeKind.Utc),
                Order = 1,
                Text = "Good story, nice and short!"
            },
            new Comment
            {
                Id = IdProvider.CommentId("Tech", "Lorem", 2),
                UserId = IdProvider.UserId("Joost"),
                CommentDate = new DateTime(2017, 6, 1, 13, 8, 0, DateTimeKind.Utc),
                Order = 2,
                Text = "Thank you for your feedback!"
            }
        };
        Blog IRead<Blog>.Read(IIdentity<Blog> id)
            => blogs.Where(b => IdProvider.Equals(b.Id, id)).FirstOrDefault();

        BlogEntry IRead<BlogEntry>.Read(IIdentity<BlogEntry> id)
            => entries.Where(e => IdProvider.Equals(e.Id, id)).FirstOrDefault();

        Comment IRead<Comment>.Read(IIdentity<Comment> id)
            => comments.Where(c => IdProvider.Equals(c.Id, id)).FirstOrDefault();

    }
}
