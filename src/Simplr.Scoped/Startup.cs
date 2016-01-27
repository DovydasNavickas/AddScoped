using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scopes
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ScopedClass>();
            services.AddScoped<ILazyCount, ScopedClass>();
            services.AddScoped<ILazyCount, ScopedClass2>();
            services.AddScoped<IPrincipalSlim, PrincipalSlim>();
            services.AddScoped<Handler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseIISPlatformHandler();

            app.Use(async (context, next) =>
            {
                var request = context.Request;
                var response = context.Response;
                var items = context.Items;

                Func<IPrincipalSlim, Task> printPrincipal = async principal =>
                {
                    await response.WriteAsync($"{principal.UserId}{Environment.NewLine}");
                    var tasks = principal.Claims?.Select(async claim => await response.WriteAsync($"  {claim.Value}{Environment.NewLine}"));
                    if (tasks != null)
                    {
                        await Task.WhenAll(tasks);
                    }
                    await response.WriteAsync($"{Environment.NewLine}");
                };

                if (request.Path == "/test1")
                {

                }
                else if (request.Path == "/test2")
                {

                }
                else if (request.Path == "/test3")
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    {
                        var principal = serviceScope.ServiceProvider.GetRequiredService<IPrincipalSlim>();

                        principal.UserId = "users-1";
                        principal.Claims = new Dictionary<string, string>
                        {
                            ["cl1"] = "Claim 1",
                            ["cl2"] = "Claim 2",
                            ["cl3"] = "Claim 3",
                        };

                        var handler = serviceScope.ServiceProvider.GetRequiredService<Handler>();
                        await printPrincipal(handler.Principal);
                    }

                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    {
                        var principal = serviceScope.ServiceProvider.GetRequiredService<IPrincipalSlim>();

                        principal.UserId = "users-2";

                        var handler = serviceScope.ServiceProvider.GetRequiredService<Handler>();
                        await printPrincipal(handler.Principal);
                    }
                }
                else
                {
                    await next();
                }
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
