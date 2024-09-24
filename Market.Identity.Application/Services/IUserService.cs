using Market.Identity.Application.Dtos;
using Market.Identity.Domain.Entities;

namespace Market.Identity.Application.Services;

public interface IUserService
{
    Task RegisterUserAsync(UserRegistrationDto userDto);
    Task<User?> AuthenticateUserAsync(LoginDto loginDto);
}