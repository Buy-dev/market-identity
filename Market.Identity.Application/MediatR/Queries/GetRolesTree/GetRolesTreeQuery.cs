using Market.Identity.Application.Infrastructure.Mappers;
using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Market.Identity.Application.MediatR.Queries.GetRolesTree;

public record GetRolesTreeQuery : IRequest<Result<RolesTreeDto>>;

public class GetRolesTreeQueryHandler(IIdentityDbContext context, RoleMapper mapper) 
    : IRequestHandler<GetRolesTreeQuery, Result<RolesTreeDto>>
{
    public async Task<Result<RolesTreeDto>> Handle(GetRolesTreeQuery request, CancellationToken cancellationToken)
    {
        var roles = await context.Roles
            .AsNoTracking()
            .ToDictionaryAsync(r => r.Name, cancellationToken)
            .ConfigureAwait(false);
        var roleGroups = new List<RoleGroupDto>
        {
            new()
            {
                ParentName = "Продавец",
                Roles = GetRoles(roles, Roles.ShopOwner)
            },
            new()
            {
                ParentName = "Покупатель",
                Roles = GetRoles(roles, Roles.Customer)
            },
        };
        var rolesTree = new RolesTreeDto(roleGroups);
        return Result<RolesTreeDto>.Success(rolesTree);
    }
    
    private List<RoleDto> GetRoles(Dictionary<string, Role> roles, params Roles[] rolesToGet)
    {
        return rolesToGet
            .Select( r => mapper.Map(roles[r.ToString()]))
            .ToList();
    }
}