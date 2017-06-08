using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Domain
{
    /// <summary>
    /// This class represents a user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets and sets the user's identity value.
        /// </summary>
        public IIdentity<User> Id { get; set; }
        /// <summary>
        /// Gets and sets the user name.
        /// This name should be short and identifier-like.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets and sets the full name.
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// Gets and sets the password-hash.
        /// </summary>
        public string Password { get; set; }
    }
}
