namespace Market.Identity.Application.MediatR.Queries.GetRolesTree;

public record RoleGroupDto
{
    public string ParentName { get; init; }
    public List<RoleDto> Roles { get; set; }
}