using System;
using System.Collections.Generic;
using System.Text;

namespace Biz.Morsink.Identity.Systems
{
    /// <summary>
    /// This class is a default implementation for IIdentity&lt;Sys&gt;.
    /// </summary>
    public class SystemIdentity : FreeIdentity<Sys, string>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="sys">The underlying value for the System identity.</param>
        public SystemIdentity(string sys) : base(null, sys) { }
    }


}
