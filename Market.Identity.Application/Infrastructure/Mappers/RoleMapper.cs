using Market.Identity.Application.MediatR.Queries.GetRolesTree;
using Market.Identity.Domain.Entities;

namespace Market.Identity.Application.Infrastructure.Mappers;

public class RoleMapper : IMapWith<Role, RoleDto>
{
    public RoleDto Map(Role role)
    {
        return new RoleDto(role.Id, role.Title, role.Description);
    }
}