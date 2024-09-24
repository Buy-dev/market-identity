using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;
using Market.Identity.Infrastucture.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Market.Identity.Infrastucture.Repositories;

public class UserRepository(IdentityDbContext context) : IUserRepository
{
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await context.Users
                             .Include(u => u.UserRoles)
                             .FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await context.Users
                             .Include(u => u.UserRoles)
                             .FirstOrDefaultAsync(u => u.UserName == username);
    }

    public async Task AddUserAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }
}