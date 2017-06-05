using Biz.Morsink.Identity.Test.WebApplication.Domain;
using Biz.Morsink.Identity.Test.WebApplication.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Repositories
{
    /// <summary>
    /// This class representsthe repository pattern for User objects.
    /// </summary>
    public class UserRepository : IRead<User>
    {
        private static BackendIdentityProvider IdProvider = BackendIdentityProvider.Instance;
        private List<User> _users = new List<User>
        {
            new User
            {
                Id = IdProvider.UserId("Joost"),
                Name = "Joost",
                FullName = "Joost Morsink",
                Password = "81fffa21c78a37f721d2aa2a9532d82f7f01c984"
            },
            new User
            {
                Id = IdProvider.UserId("guest"),
                Name = "Guest",
                FullName ="Anonymous guest user",
                Password =  ""
            }
        };
        /// <summary>
        /// Tries to find a user in this repository.
        /// </summary>
        /// <param name="id">The identity value for the user.</param>
        /// <returns>A User object if one can be found in this repository, null otherwise.</returns>
        public User Read(IIdentity<User> id)
            => _users.Where(u => IdProvider.Equals(u.Id, id)).FirstOrDefault();

    }
}
