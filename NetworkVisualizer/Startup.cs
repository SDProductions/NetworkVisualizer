using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
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

            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            services.Configure<OpenIdConnectOptions>(AzureADDefaults.OpenIdScheme, options =>
            {
                options.Authority = options.Authority + "/v2.0/";

                // Per the code below, this application signs in users in any Work and School
                // accounts and any Microsoft Personal Accounts.
                // If you want to direct Azure AD to restrict the users that can sign-in, change 
                // the tenant value of the appsettings.json file in the following way:
                // - only Work and School accounts => 'organizations'
                // - only Microsoft Personal accounts => 'consumers'
                // - Work and School and Personal accounts => 'common'

                // If you want to restrict the users that can sign-in to only one tenant
                // set the tenant value in the appsettings.json file to the tenant ID of this
                // organization, and set ValidateIssuer below to true.

                // If you want to restrict the users that can sign-in to several organizations
                // Set the tenant value in the appsettings.json file to 'organizations', set
                // ValidateIssuer, above to 'true', and add the issuers you want to accept to the
                // options.TokenValidationParameters.ValidIssuers collection
                options.TokenValidationParameters.ValidateIssuer = false;
            });

            services.AddMvc(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            })
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<NetworkVisualizerContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("NetworkVisualizerContext")));

            services.AddHostedService<PruneDatabaseService>();
            services.AddHostedService<UpdateGraphsService>();
            services.AddHostedService<DataGenerationService>();
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
            app.UseAuthentication();

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
                // Admin control panel options
                routes.MapRoute(
                    "config",
                    "config",
                    new { controller = "Admin", action = "Config", id = "" });
                routes.MapRoute(
                    "packets",
                    "packets",
                    new { controller = "Admin", action = "Packets", id = "" });
                routes.MapRoute(
                    "audit",
                    "audit",
                    new { controller = "Admin", action = "Audit", id = "" });
                routes.MapRoute(
                    "account",
                    "account",
                    new { controller = "Admin", action = "Account", id = "" });
                routes.MapRoute(
                    "help",
                    "help",
                    new { controller = "Admin", action = "Help", id = "" });

                // Routes for Cache
                routes.MapRoute(
                    "cache",
                    "cache",
                    new { controller = "Caches", action = "Index", id = "" });
            });
        }
    }
}
