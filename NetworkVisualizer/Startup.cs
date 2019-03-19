using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using NetworkVisualizer.Models;
using NetworkVisualizer.Services;

namespace NetworkVisualizer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.production.json", optional: true)
                .AddEnvironmentVariables();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<NetworkVisualizerContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("NetworkVisualizerContext")));

            services.AddHostedService<PruneDatabaseService>();
            services.AddHostedService<UpdateGraphsService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                // Routes for Home
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    "about",
                    "about",
                    new { controller = "Home", action = "About", id = "" });
                routes.MapRoute(
                    "terms",
                    "terms",
                    new { controller = "Home", action = "Terms", id = "" });

                // Routes for Admin
                routes.MapRoute(
                    "admin",
                    "admin",
                    new { controller = "Admin", action = "Index", id = "" });
                routes.MapRoute(
                    "login",
                    "login",
                    new { controller = "Admin", action = "Login", id = "" });
                routes.MapRoute(
                    "packets",
                    "packets",
                    new { controller = "Admin", action = "PacketList", id = "" });
                routes.MapRoute(
                    "users",
                    "users",
                    new { controller = "Admin", action = "UserList", id = "" });

                // Routes for Cache
                routes.MapRoute(
                    "cache",
                    "cache",
                    new { controller = "Caches", action = "Index", id = "" });
            });
        }
    }
}
