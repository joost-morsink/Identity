using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Domain
{
    public class BlogEntry
    {
        public IIdentity<Blog, BlogEntry> Id { get; set; }
        public string Title { get; set;}
        public IIdentity<User> AuthorId { get; set; }
        public DateTime PostDate { get; set; }
        public string Text { get; set; }

    }
}
