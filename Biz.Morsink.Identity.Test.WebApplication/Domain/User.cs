using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Domain
{
    public class User
    {
        public IIdentity<User> Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
    }
}
