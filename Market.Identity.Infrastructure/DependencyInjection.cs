using System.Reflection;
using Market.Identity.Application.Services;
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
        services.AddDbContextPool<IIdentityDbContext, IdentityDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("IdentityDbConnection")), 10)
            .AddScoped<ITokenService, TokenService>()
            .AddRepositories();

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        var types = Assembly.GetExecutingAssembly()
            .GetTypes();
        var repositoryInterfaces = types.Where(t => t is { IsInterface: true, Name: "IRepository`1" });
        foreach (var repositoryInterface in repositoryInterfaces)
        {
            var entityType = repositoryInterface.GetGenericArguments().First();
            var implementationType = typeof(Repository<>).MakeGenericType(entityType);
            services.AddScoped(repositoryInterface, implementationType);
        }

        return services;
    }
}