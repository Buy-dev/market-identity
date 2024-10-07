using System.Reflection;
using FluentValidation;
using Market.Identity.Application.Infrastructure.Mappers;
using Market.Identity.Application.Infrastructure.Validation;
using Market.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Enums;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace Market.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
                    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
                .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly())
                .AddFluentValidationAutoValidation(configuration =>
                {
                    configuration.DisableBuiltInModelValidation = true;
                    configuration.ValidationStrategy = ValidationStrategy.All;
                    configuration.EnableBodyBindingSourceAutomaticValidation = true;
                    configuration.EnableFormBindingSourceAutomaticValidation = true;
                    configuration.EnableQueryBindingSourceAutomaticValidation = true;
                    configuration.EnablePathBindingSourceAutomaticValidation = true;
                    configuration.EnableCustomBindingSourceAutomaticValidation = true;
                    configuration.OverrideDefaultResultFactoryWith<CustomResultFactory>();
                })
                .AddMappers()
                .AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        
        return services;
    }

    private static IServiceCollection AddGenericService(this IServiceCollection services, Type type)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var a = assembly.GetTypes();
        var mappers = assembly.GetTypes()
            .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapWith<,>)));

        foreach (var mapper in mappers)
        {
            services.AddScoped(mapper);
        }

        return services;
    }
    
    private static IServiceCollection AddMappers(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var a = assembly.GetTypes();
        var mappers = assembly.GetTypes()
          .Where(type => type.IsClass && !type.IsAbstract && type.GetInterfaces()
              .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IMapWith<,>)));

        foreach (var mapper in mappers)
        {
            services.AddScoped(mapper);
        }

        return services;
    }
}