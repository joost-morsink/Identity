using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Biz.Morsink.Identity.Test.WebApplication.Repositories;
using Biz.Morsink.Identity.Test.WebApplication.Generic;
using Biz.Morsink.Identity.Test.WebApplication.Domain;

namespace Biz.Morsink.Identity.Test.WebApplication
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IRead<User>, UserRepository>();
            services.AddSingleton<IRead<Blog>, BlogRepository>();
            services.AddSingleton<IRead<BlogEntry>, BlogRepository>();
            services.AddSingleton<IRead<Comment>, BlogRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<IdentityBasedResources>();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("This should never occur.");
            });
        }
    }
}
