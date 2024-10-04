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
                .AddScoped<ITokenService, TokenService>();

        return services;
    }
}