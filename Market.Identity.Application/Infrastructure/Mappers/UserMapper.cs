using Market.Identity.Application.Dtos;
using Market.Identity.Domain.Entities;

namespace Market.Identity.Application.Infrastructure.Mappers;

public class UserMapper : IMapWith<User, UserDto>
{
    public UserDto Map(User user)
    {
        return new UserDto(
            user.Id, 
            user.Username, 
            user.PasswordHash, 
            user.UserRoles.Select(ur => ur.Role.Name).ToList()
        );
    }
}