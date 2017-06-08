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
    /// <summary>
    /// The ASP.Net Core Startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Configure dependency injection.
        /// </summary>
        /// <param name="services">Injected IServiceCollection.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IRead<User>, UserRepository>();
            services.AddSingleton<IRead<Blog>, BlogRepository>();
            services.AddSingleton<IRead<BlogEntry>, BlogRepository>();
            services.AddSingleton<IRead<Comment>, BlogRepository>();
        }

        /// <summary>
        /// Configures the HTTP request pipeline.
        /// </summary>
        /// <param name="app">Injected IApplicationBuilder.</param>
        /// <param name="env">Injected IHostingEnvironment.</param>
        /// <param name="loggerFactory">Injected ILoggerFactory.</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            // Insert a middleware component for handling the root path.
            app.Use(async (context, next) => {
                if (context.Request.Path == "/")
                    await context.Response.WriteAsync(HOME);
                else
                    await next();
            });
            // Insert the example middleware into the pipeline.
            app.UseIdentityBaseResources();
            
            // This component should never run, because the IdentityBasedResourceMiddleware component never calls 'next'.
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("This should never occur.");
            });
        }
        /// <summary>
        /// This constant contains an HTML page with links to the json resources this example API produces.
        /// </summary>
        public const string HOME = @"
<html>
<head><title>Example homepage.</title></head>
<body>
<h1>Identity-based resources example API</h1>
<p>This API contains the following readonly resources:</p>
<ul>
    <li>User</li>
    <ul>
        <li><a href=""/user/Joost"">Joost</a></li>
        <li><a href=""/user/guest"">Guest</a></li>
    </ul>
    <li>Blog</li>
    <ul>
        <li><a href=""/blog/Tech"">Joost's technology blog</a></li>
        <ul>
            <li>Blog entry: <a href=""/blog/Tech/Lorem"">Lorem ipsum dolor...</a></li>
            <ul>
                <li><a href=""/blog/Tech/Lorem/comments/1"">Comment by guest</a></li>
                <li><a href=""/blog/Tech/Lorem/comments/2"">Reply by Joost</a></li>
            </ul>
        </ul>
    </ul>
</ul>
</html>
";
    }
}
