using System.Security.Claims;
using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;
using Market.Identity.Domain.Entities.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Market.Identity.Infrastructure.Persistence;

public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options), IIdentityDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        modelBuilder.SeedRoles();
    }
    
    public async Task<bool> SaveAsync(CancellationToken cancellationToken = default)
    {
        var userId = Guid.Empty;
        var httpContextAccessor = this.GetService<IHttpContextAccessor>();
        var userIdVal = httpContextAccessor.HttpContext?
            .User?
            .FindFirstValue(ClaimTypes.NameIdentifier);

        if (!string.IsNullOrEmpty(userIdVal))
            userId = new Guid(userIdVal);

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.Created = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.ModifiedBy = userId;
                    entry.Entity.Modified = DateTime.UtcNow;
                    break;
            }
        }
        
        try
        {
            await base.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch(Exception e)
        {
            return false;
        }
    }

    public void AttachEntityIfNeeded<T>(T entity) where T : class
    {
        var entityEntry = Entry(entity);
        if(entityEntry.State == EntityState.Detached)
            Set<T>().Attach(entity);
    }
}