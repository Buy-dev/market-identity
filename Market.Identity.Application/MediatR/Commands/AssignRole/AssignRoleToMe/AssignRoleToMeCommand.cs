using FluentValidation;
using Market.Identity.Application.Dtos;
using Market.Identity.Application.Infrastructure.Mappers;
using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Market.Identity.Application.MediatR.Commands.AssignRole.AssignRoleToMe;

public class AssignRoleToMeCommand : IRequest<Result<string>>
{
    public long RoleId { get; set; }
}

public class AssignRoleToMeCommandHandler(
    IIdentityDbContext dbContext,
    ShortenUserMapper mapper,
    ValidationContext<AssignRoleToMeCommand> validationContext)
    : IRequestHandler<AssignRoleToMeCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AssignRoleToMeCommand request, CancellationToken cancellationToken)
    {
        var user = validationContext.RootContextData["User"] as ShortenUserDto;
        var userRole = new UserRole()
        {
            UserId = user.Id
        }

        return Result<string>.Success("Role assigned successfully");
    }
}