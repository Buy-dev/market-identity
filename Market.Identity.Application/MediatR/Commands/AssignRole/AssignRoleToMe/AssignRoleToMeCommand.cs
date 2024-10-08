using FluentValidation;
using Market.Identity.Application.Dtos;
using Market.Identity.Application.Infrastructure.Mappers;
using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Market.Identity.Application.MediatR.Commands.AssignRole.AssignRoleToMe;

public class AssignRoleToMeCommand : IRequest<Result<object>>
{
    public long RoleId { get; set; }
}

public class AssignRoleToMeCommandHandler(
    IRepository<UserRole> userRoleRepository,
    IRepository<User> userRepository,
    ICurrentUserService currentUserService,
    ShortenUserMapper mapper)
    : IRequestHandler<AssignRoleToMeCommand, Result<object>>
{
    public async Task<Result<object>> Handle(AssignRoleToMeCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository
            .GetByAndMapAsync(u => u.Username == currentUserService.Username, mapper, cancellationToken)
            .ConfigureAwait(false);
        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = request.RoleId
        };

        await userRoleRepository.AddAsync(userRole, cancellationToken);

        return Result<object>.Success();
    }
}