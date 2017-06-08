using Biz.Morsink.Identity.PathProvider;
using Biz.Morsink.Identity.Test.WebApplication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Biz.Morsink.DataConvert;

namespace Biz.Morsink.Identity.Test.WebApplication.IdentityProvider
{
    /// <summary>
    /// The ApiIdentityProvider extens the PathIdentityProvider.
    /// It contains path mappings for User, Blog, BlogEntry and Comment types.
    /// </summary>
    public class ApiIdentityProvider : PathIdentityProvider
    {
        private static DataConverter _converter = Converters.WithSeparator('-');

        /// <summary>
        /// A singleton instance.
        /// </summary>
        public static ApiIdentityProvider Instance { get; } = new ApiIdentityProvider();
        /// <summary>
        /// Constructor.
        /// </summary>
        public ApiIdentityProvider()
        {
            AddEntry("/user/*", typeof(User));
            AddEntry("/blog/*", typeof(Blog));
            BuildEntry(typeof(Blog), typeof(BlogEntry))
                .WithPath("/blog/*/*")
                .WithPath("/entry/*", 1)
                .Add();
            BuildEntry(typeof(Blog), typeof(BlogEntry), typeof(Comment))
                .WithPath("/blog/*/*/comments/*")
                .WithPath("/comment/*", 1)
                .Add();
        }
        /// <summary>
        /// Always returns the default converter.
        /// The default converter is the main default with a SeparatedStringConverter for '-' added.
        /// </summary>
        /// <param name="t">Ignored.</param>
        /// <param name="incoming">Ignored.</param>
        /// <returns>A default IDataConverter instance for ApiIdentityProvider.</returns>
        public override IDataConverter GetConverter(Type t, bool incoming)
            => _converter;
    }
}
