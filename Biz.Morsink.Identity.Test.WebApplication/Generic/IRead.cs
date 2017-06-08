using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Biz.Morsink.Identity.Test.WebApplication.Generic
{
    /// <summary>
    /// Example interface to model a service that can retrieve entities by identity values.
    /// </summary>
    /// <typeparam name="T">The type of entity to retrieve.</typeparam>
    public interface IRead<T>
        where T : class
    {
        /// <summary>
        /// Retrieves an entity.
        /// </summary>
        /// <param name="id">The identity value of the entity.</param>
        /// <returns>The entity if one exists with the given identity value, null otherwise.</returns>
        T Read(IIdentity<T> id);
    }
}
