using Market.Identity.Application.Repositories;
using Market.Identity.Domain.Entities;
using Market.Identity.Infrastucture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Market.Identity.Infrastucture.Repositories;

public class RefreshTokenRepository(IdentityDbContext context) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken refreshToken)
    {
        await context.RefreshTokens.AddAsync(refreshToken);
        await context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await context.RefreshTokens
                             .FirstOrDefaultAsync(t => t.Token == token);
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        context.RefreshTokens.Update(refreshToken);
        await context.SaveChangesAsync();
    }
}