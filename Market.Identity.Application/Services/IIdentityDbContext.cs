using Market.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Identity.Application.Services;

public interface IIdentityDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    public Task<bool> SaveAsync(CancellationToken cancellationToken = default);
    
    public void AttachEntityIfNeeded<T>(T entity) where T : class;
    
    public DbSet<T> Set<T>() where T : class;
}