using System.Reflection;
using FluentValidation;
using FluentValidation.Results;
using Market.Identity.Application.Helpers;
using Market.Identity.Application.Infrastructure.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace Market.Identity.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        
        services.AddScoped<IMapWith<ValidationFailure, ValidationError>, ValidationErrorMapper>();
        return services;
    }
}