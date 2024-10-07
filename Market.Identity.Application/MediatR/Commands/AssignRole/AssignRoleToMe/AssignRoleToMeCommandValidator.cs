using FluentValidation;
using FluentValidation.Results;
using Market.Identity.Application.Infrastructure.Mappers;
using Market.Identity.Application.Infrastructure.Validation;
using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Market.Identity.Application.MediatR.Commands.AssignRole.AssignRoleToMe;

public class AssignRoleToMeCommandValidator : BaseValidator<AssignRoleToMeCommand>
{
    private readonly IIdentityDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ShortenUserMapper _mapper;

    public AssignRoleToMeCommandValidator(
        IIdentityDbContext dbContext,
        ICurrentUserService currentUserService,
        ShortenUserMapper mapper)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _mapper = mapper;

        RuleFor(v => v.RoleId)
            .NotNull().WithMessage("Роль обязательна")
            .MustAsync(BeExistingRole).WithMessage("Такой роли не существует");
    }

    public override async Task<ValidationResult> ValidateAsync(
        ValidationContext<AssignRoleToMeCommand> context, 
        CancellationToken cancellation = new())
    {
        var validationResult = await base.ValidateAsync(context, cancellation) 
                               ?? new ValidationResult();
        var user = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Username == _currentUserService.Username)
            .Select(u => _mapper.Map(u))
            .FirstOrDefaultAsync(cancellation).ConfigureAwait(false);

        if (user == null) 
            return GenerateValidationResultWithFailure(validationResult, "Пользователь не найден")
        
        var isRelationPresent = _dbContext.UserRoles
            .AsNoTracking()
            .Any(ur => ur.UserId == user.Id && ur.RoleId == context.InstanceToValidate.RoleId);

        if (!isRelationPresent)
            return GenerateValidationResultWithFailure(validationResult, "Роль уже назначена");

        context.RootContextData["User"] = user;

        return validationResult;
    }

    private Task<bool> BeExistingRole(long roleId, CancellationToken cancellationToken)
        => _dbContext.Roles.AsNoTracking()
            .AnyAsync(u => u.Id == roleId, cancellationToken);
}