using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using CourtApp.Application.Extensions;
using CourtApp.Application.Interfaces.Shared;
using CourtApp.Infrastructure.Extensions;
using CourtApp.Web.Abstractions;
using CourtApp.Web.Extensions;
using CourtApp.Web.Helpers;
using CourtApp.Web.Models;
using CourtApp.Web.Permission;
using CourtApp.Web.Services;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;

namespace CourtApp.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IConfiguration _configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.

        [Obsolete]
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
            services.AddNotyf(o =>
            {
                o.DurationInSeconds = 10;
                o.IsDismissable = true;
                o.HasRippleEffect = true;
            });
            services.AddApplicationLayer();
            
            services.AddPersistenceContexts(_configuration);
            services.AddRepositories();
            services.AddSharedInfrastructure(_configuration);
            services.AddMultiLingualSupport();

            services.AddFluentValidationAutoValidation();
            services.AddControllersWithViews().AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            });
            services.AddMvc().AddJsonOptions(options =>
             {
                 options.JsonSerializerOptions.PropertyNamingPolicy = null;
             });
            services.Configure<FormOptions>(opt =>
            {
                opt.MultipartBodyLengthLimit = 512 * 1024 * 1024; // 512 MB limit
            });
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = 200 * 1024 * 1024; // 200MB
            });
            services.Configure<KestrelServerOptions>(options =>
            {
                // Example: Setting a custom maximum request body size (for large file uploads)
                options.Limits.MaxRequestBodySize = 200 * 1024 * 1024; // 200MB

                // Example: Setting the timeout for connections
                options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
            });
            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
            services.AddDistributedMemoryCache();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20); // Set timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
                options.SlidingExpiration = true;
                options.LoginPath = new PathString("/Identity/Account/Login"); // ✅ Correct path for areas
                options.AccessDeniedPath = new PathString("/Identity/Account/AccessDenied"); // ✅ Access Denied Page
            });

            services.AddSingleton<BlobService>();
            services.Configure<UploadSettings>(_configuration.GetSection("UploadSettings"));
            var uploadProvider = _configuration["UploadSettings:Provider"];
            if (uploadProvider == "Azure")
            {
                services.AddScoped<IDocumentUploadService, AzureBlobUploaderService>();
            }
            else if (uploadProvider == "GoogleDrive")
            {
                services.AddScoped<IDocumentUploadService, GoogleDriveUploaderService>();
            }
            else
            {
                services.AddScoped<IDocumentUploadService, LocalUploaderService>();
            }
            services.AddScoped<ActionRenderHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMultiLingualFeature();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts(); // Keep HSTS enabled in production
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();     // Ensure cookies respect policy
            app.UseRouting();
            app.UseSession();          // 🔑 Session before Routing


            app.UseAuthentication();   // 🔒 Authentication before Authorization
            app.UseAuthorization();    // 🔐 Authorization after Authentication

            app.UseNotyf(); // Notification (can also be placed earlier if not dependent on auth/session)

            // Routing setup
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area=Dashboard}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

        }
    }
}