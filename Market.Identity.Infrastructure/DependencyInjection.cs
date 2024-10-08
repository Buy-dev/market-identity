using System.Reflection;
using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;
using Market.Identity.Infrastructure.Persistence;
using Market.Identity.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Market.Identity.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddScoped<ITokenService, TokenService>()
            .AddScoped(typeof(IRepository<>), typeof(Repository<>))
            .AddScoped<ICurrentUserService, CurrentUserService>()
            .AddDbContextPool<IIdentityDbContext, IdentityDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("IdentityDbConnection")), 10);

        return services;
    }
    
    private static void RegisterRepositories(this IServiceCollection services)
    {
        var repositoryInterfaceType = typeof(IRepository<>);
        var repositoryImplementationType = typeof(Repository<>);

        var assembly = Assembly.GetExecutingAssembly();
        var types = assembly.GetTypes();

        foreach (var type in types)
        {
            if (!type.IsClass || type.IsAbstract)
                continue;

            var interfaces = type.GetInterfaces();
            foreach (var @interface in interfaces)
            {
                if (@interface.IsGenericType && @interface.GetGenericTypeDefinition() == repositoryInterfaceType)
                {
                    var entityType = @interface.GetGenericArguments()[0];
                    var implementationType = repositoryImplementationType.MakeGenericType(entityType);
                    services.AddScoped(@interface, implementationType);
                }
            }
        }
    }
}