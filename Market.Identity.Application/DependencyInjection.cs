using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Market.Identity.Application.Helpers;
using Market.Identity.Application.Infrastructure.Mappers;
using Market.Identity.Application.Infrastructure.Validation;
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
                });
        
        return services;
    }
}