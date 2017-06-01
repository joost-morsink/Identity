using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Domain
{
    public class Blog
    {
        public IIdentity<Blog> Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }

    }
}
