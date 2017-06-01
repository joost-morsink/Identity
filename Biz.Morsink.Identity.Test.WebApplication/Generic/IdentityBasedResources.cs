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

namespace Biz.Morsink.Identity.Test.WebApplication.Generic
{
    public class IdentityBasedResources
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

        public IdentityBasedResources(RequestDelegate next, IServiceProvider serviceProvider)
        {
            this.next = next;
            this.serviceProvider = serviceProvider;
            this.serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new ContractResolver()
            };
        }
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
}
