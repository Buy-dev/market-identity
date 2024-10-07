using Market.Identity.Application.Infrastructure.Mappers;
using Market.Identity.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Market.Identity.Application.MediatR.Commands.AssignRole.AssignRoleToMe;

public class AssignRoleToMeCommand : IRequest<Result<string>>
{
    public long RoleId { get; set; }
}

public class AssignRoleToMeCommandHandler(
    IIdentityDbContext dbContext,
    ShortenUserMapper mapper)
    : IRequestHandler<AssignRoleToMeCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AssignRoleToMeCommand request, CancellationToken cancellationToken)
    {
        request.Validation

        return Result<string>.Success("Role assigned successfully");
    }
}