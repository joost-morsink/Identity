using Biz.Morsink.Identity.Test.WebApplication.Domain;
using Biz.Morsink.Identity.Test.WebApplication.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Repositories
{
    public class UserRepository : IRead<User>
    {
        private static BackendIdentityProvider IdProvider = BackendIdentityProvider.Instance;
        public UserRepository()
        {

        }
        private List<User> _users = new List<User>
        {
            new User
            {
                Id = IdProvider.UserId("Joost"),
                Name = "Joost",
                FullName = "Joost Morsink",
                Password = "Welcome01"
            },
            new User
            {
                Id = IdProvider.UserId("guest"),
                Name = "Guest",
                FullName ="Anonymous guest user",
                Password =  ""
            }
        };

        public User Read(IIdentity<User> id)
            => _users.Where(u => IdProvider.Equals(u.Id, id)).FirstOrDefault();

    }
}
