using Market.Identity.Application.Helpers;
using Market.Identity.Application.MediatR.Commands.LoginUser;
using Market.Identity.Domain.Entities;

namespace Market.Identity.Application.Infrastructure.Mappers;

public class LoginUserMapper : IMapWith<User, LoginUserDto>
{
    public LoginUserDto Map(User user)
    {
        return new LoginUserDto()
        {
            Id = user.Id,
            Username = user.Username,
            PasswordHash = user.PasswordHash,
            Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
        };
    }
}