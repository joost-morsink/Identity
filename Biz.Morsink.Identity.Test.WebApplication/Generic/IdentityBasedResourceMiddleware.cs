using Biz.Morsink.Identity.Test.WebApplication.IdentityProvider;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Builder;

namespace Biz.Morsink.Identity.Test.WebApplication.Generic
{
    /// <summary>
    /// This component is an ASP.Net Core middleware component and implements the core idea of the identity library.
    /// It uses an ApiIdentityProvider to parse the request path.
    /// If it is successful the identity value's ForType property won't be Object.
    /// The type can be used to construct a service type to read such objects (IRead&lt;T&gt;, with typeof(T) == id.ForType).
    /// It then invokes the service to retrieve the object and serializes it to JSON before sending it in the response.
    /// A specific converter is used for properties that contain identity values to output the path, again by using ghe ApiIdentityProvider
    /// </summary>
    public class IdentityBasedResourceMiddleware
    {
        private class ContractResolver : CamelCasePropertyNamesContractResolver
        {
            protected override JsonContract CreateContract(Type objectType)
            {
                var bc =  base.CreateContract(objectType);
                if (typeof(IIdentity).GetTypeInfo().IsAssignableFrom(objectType))
                    bc.Converter = new IdentityConverter();
                return bc;
            }

        }
        private readonly RequestDelegate next;
        private readonly IServiceProvider serviceProvider;
        private readonly JsonSerializerSettings serializerSettings;

        /// <summary>
        /// Constructor for middleware.
        /// </summary>
        /// <param name="next">The next delegate, which is never used.</param>
        /// <param name="serviceProvider">The ASP.Net Core service provider</param>
        public IdentityBasedResourceMiddleware(RequestDelegate next, IServiceProvider serviceProvider)
        {
            this.next = next;
            this.serviceProvider = serviceProvider;
            this.serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new ContractResolver()
            };
        }
        /// <summary>
        /// Invokes the middleware component.
        /// </summary>
        /// <param name="context">The HttpContext for the request.</param>
        /// <returns>A Task representing the work to be done.</returns>
        public Task Invoke(HttpContext context)
        {
            var reqpath = context.Request.Path;
            var res = ApiIdentityProvider.Instance.Parse(reqpath);
            if (res.ForType != typeof(object))
            {
                object srv = serviceProvider.GetService(typeof(IRead<>).MakeGenericType(res.ForType));
                if (srv == null)
                    context.Response.StatusCode = 400;
                else
                {
                    object o = typeof(IRead<>).MakeGenericType(res.ForType).GetTypeInfo().GetDeclaredMethod(nameof(IRead<object>.Read)).Invoke(srv, new object[] { res });
                    if (o == null)
                        context.Response.StatusCode = 404;
                    else
                    {
                        context.Response.StatusCode = 200;
                        var ser = new JsonSerializer() { ContractResolver = new ContractResolver() };
                        using (var strwri = new StreamWriter(context.Response.Body))
                        using (var wri = new JsonTextWriter(strwri))
                            ser.Serialize(wri, o);   
                    }
                }
            }
            else
                context.Response.StatusCode = 404;
            return Task.FromResult(0);
        }
    }
    public static class IdentityBasedResourceMiddlewareExt
    {
        public static IApplicationBuilder UseIdentityBaseResources(this IApplicationBuilder appBuilder)
            => appBuilder.UseMiddleware<IdentityBasedResourceMiddleware>();

    }
}
