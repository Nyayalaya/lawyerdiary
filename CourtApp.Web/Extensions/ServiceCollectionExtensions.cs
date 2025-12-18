using CourtApp.Application.DTOs.Settings;
using CourtApp.Application.Interfaces.Shared;
using CourtApp.Infrastructure.DbContexts;
using CourtApp.Infrastructure.Identity.Models;
using CourtApp.Infrastructure.Shared.Services;
using CourtApp.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Globalization;

namespace CourtApp.Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddMultiLingualSupport(this IServiceCollection services)
        {
            #region Registering ResourcesPath

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            #endregion Registering ResourcesPath

            services.AddMvc()
               .AddViewLocalization(Microsoft.AspNetCore.Mvc.Razor
               .LanguageViewLocationExpanderFormat.Suffix)
               .AddDataAnnotationsLocalization(options =>
               {
                   options.DataAnnotationLocalizerProvider = (type, factory) =>
                       factory.Create(typeof(SharedResource));
               });
            services.AddRouting(o => o.LowercaseUrls = true);
            services.AddHttpContextAccessor();
            services.Configure<RequestLocalizationOptions>(options =>
            {
                var cultures = new List<CultureInfo> {
                    new CultureInfo("en"),
                    new CultureInfo("hi"),
                    //new CultureInfo("gu"),
                };
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
                options.SupportedCultures = cultures;
                options.SupportedUICultures = cultures;
            });
        }

        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddPersistenceContexts(configuration);
            services.AddAuthenticationScheme(configuration);
        }

        private static void AddAuthenticationScheme(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMvc(o =>
            {
                //Add Authentication to all Controllers by default.
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                o.Filters.Add(new AuthorizeFilter(policy));
            });
        }

        private static void AddPersistenceContexts(this IServiceCollection services, IConfiguration configuration)
        {
            if (configuration.GetValue<bool>("UseInMemoryDatabase"))
            {
                services.AddDbContext<IdentityContext>(options => options.UseInMemoryDatabase("IdentityDb"), ServiceLifetime.Transient);
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("ApplicationDb"));
            }
            else
            {
                services.AddDbContext<IdentityContext>(options => options.UseNpgsql(configuration.GetConnectionString("Postgres")), ServiceLifetime.Transient);
                services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Postgres")));
            }
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<IdentityContext>().AddDefaultUI().AddDefaultTokenProviders();
        }

        public static void AddSharedInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));
            services.Configure<WhatsUpSettings>(configuration.GetSection("WhatsAppSettings"));
            services.AddTransient<IDateTimeService, SystemDateTimeService>();
            services.AddTransient<IMailService, SMTPMailService>();
            services.AddTransient<WhatsAppService>();
            services.AddTransient<IAuthenticatedUserService, AuthenticatedUserService>();
            services.AddTransient<IDocumentUploadService, GoogleDriveUploaderService>();
        }
    }
}