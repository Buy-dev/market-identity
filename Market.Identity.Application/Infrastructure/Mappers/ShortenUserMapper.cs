using Market.Identity.Application.Dtos;
using Market.Identity.Domain.Entities;

namespace Market.Identity.Application.Infrastructure.Mappers;

public class ShortenUserMapper : IMapWith<User, ShortenUserDto>
{
    public ShortenUserDto Map(User user)
    {
        return new ShortenUserDto(user.Id, user.Username);
    }
}