using AutoMapper;
using CourtApp.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace CourtApp.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // AutoMapper
            services.AddAutoMapper(cfg => cfg.AddMaps(assembly), assembly);

            // FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            // MediatR
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssembly(assembly);
                config.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            return services;
        }
    }
}
//using FluentValidation;
//using Microsoft.Extensions.DependencyInjection;
//using System.Reflection;

//namespace CourtApp.Application.Extensions
//{
//    public static class ServiceCollectionExtensions
//    {
//        public static void AddApplicationLayer(this IServiceCollection services)
//        {
//            services.AddAutoMapper(cfg => { }, Assembly.GetExecutingAssembly());
//            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

//            services.AddMediatR(cfg =>
//            {
//                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
//            });
//        }
//    }
//}