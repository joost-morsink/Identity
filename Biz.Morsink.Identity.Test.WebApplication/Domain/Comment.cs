using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Domain
{
    public class Comment
    {
        public IIdentity<Blog, BlogEntry, Comment> Id { get; set; }
        public int Order { get; set; }
        public IIdentity<User> UserId { get; set; }
        public DateTime CommentDate { get; set; }
        public string Text { get; set; }
    }
}
