using Market.Identity.Domain.Entities;

namespace Market.Identity.Application.Services;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task AddUserAsync(User user);

    Task<User?> GetUserByUsernameAsync(string username);
}