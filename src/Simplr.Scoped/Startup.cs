using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
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
                if (request.Path == "/test1")
                {
                    var scopedClass = context.RequestServices.GetRequiredService<ScopedClass>();
                    await response.WriteAsync($"{scopedClass.LazyCount.Value}{Environment.NewLine}");

                    var scopedClass2 = context.RequestServices.GetRequiredService<ScopedClass>();
                    await response.WriteAsync($"{scopedClass2.LazyCount.Value}{Environment.NewLine}");
                }
                else if (request.Path == "/test2")
                {
                    var scopedClass = context.RequestServices.GetRequiredService<ScopedClass>();
                    await response.WriteAsync($"{scopedClass.LazyCount.Value}{Environment.NewLine}");

                    var x = StaticContainer.Count;
                    var y = StaticContainer.Count;

                    var scopedClass2 = context.RequestServices.GetRequiredService<ScopedClass>();
                    await response.WriteAsync($"{scopedClass2.LazyCount.Value}{Environment.NewLine}");
                }
                else if (request.Path == "/test3")
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    {

                        var scopedClass = serviceScope.ServiceProvider.GetRequiredService<ScopedClass>();
                        await response.WriteAsync($"{scopedClass.LazyCount.Value}{Environment.NewLine}");

                        var scopedClass2 = serviceScope.ServiceProvider.GetRequiredService<ScopedClass>();
                        await response.WriteAsync($"{scopedClass2.LazyCount.Value}{Environment.NewLine}");
                    }

                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                    {

                        var iServiceProvider = serviceScope.ServiceProvider;//app.ApplicationServices;
                        var services = iServiceProvider.GetServices<ILazyCount>();
                        var iLazyCount = iServiceProvider.GetRequiredService<ILazyCount>();

                        var tasks = services.Select(async x => await response.WriteAsync($"ILazyCount: {x.LazyCount.Value}{Environment.NewLine}"));
                        await Task.WhenAll(tasks);
                        await response.WriteAsync($"{iLazyCount.LazyCount.Value}{Environment.NewLine}");


                        var scopedClass = serviceScope.ServiceProvider.GetRequiredService<ScopedClass>();
                        await response.WriteAsync($"{scopedClass.LazyCount.Value}{Environment.NewLine}");

                        var scopedClass2 = serviceScope.ServiceProvider.GetRequiredService<ScopedClass>();
                        await response.WriteAsync($"{scopedClass2.LazyCount.Value}{Environment.NewLine}");
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
