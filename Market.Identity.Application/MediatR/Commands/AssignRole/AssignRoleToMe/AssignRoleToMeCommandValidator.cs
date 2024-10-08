using FluentValidation;
using FluentValidation.Results;
using Market.Identity.Application.Infrastructure.Mappers;
using Market.Identity.Application.Infrastructure.Validation;
using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;

namespace Market.Identity.Application.MediatR.Commands.AssignRole.AssignRoleToMe;

public class AssignRoleToMeCommandValidator : BaseValidator<AssignRoleToMeCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ShortenUserMapper _mapper;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<UserRole> _userRoleRepository;
    private readonly IRepository<Role> _roleRepository;

    public AssignRoleToMeCommandValidator(
        IRepository<User> userRepository,
        IRepository<UserRole> userRoleRepository,
        IRepository<Role> roleRepository,
        ICurrentUserService currentUserService,
        ShortenUserMapper mapper)
    {
        _userRepository = userRepository;
        _userRoleRepository = userRoleRepository;
        _roleRepository = roleRepository;
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
        var user = await _userRepository
            .GetByAndMapAsync(u => u.Username == _currentUserService.Username, _mapper, cancellation)
            .ConfigureAwait(false);
        
        if (user == null)
            return GenerateValidationResultWithFailure(validationResult, "Пользователь не найден");
        
        var isRelationPresent = await _userRoleRepository
            .AnyAsync(ur => ur.UserId == user.Id && ur.RoleId == context.InstanceToValidate.RoleId, cancellation)
            .ConfigureAwait(false);

        return isRelationPresent 
            ? GenerateValidationResultWithFailure(validationResult, "Роль уже назначена") 
            : validationResult;
    }

    private Task<bool> BeExistingRole(long roleId, CancellationToken cancellationToken)
        => _roleRepository
        .AnyAsync(u => u.Id == roleId, cancellationToken);
}