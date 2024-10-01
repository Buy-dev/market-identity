using FluentValidation;
using Market.Identity.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Market.Identity.Application.MediatR.Commands.LoginUser;

public class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    private readonly IIdentityDbContext _context;

    public LoginUserCommandValidator(IIdentityDbContext context)
    {
        _context = context;
        
        RuleFor(v => v.Username)
            .NotNull().WithMessage("Никнейм обязателен")
            .MinimumLength(2).WithMessage("Никнейм должен содержать не менее 2 символов")
            .MustAsync(BeExistingUser).WithMessage("Пользователя с таким никнеймом не существует");
    }

    private Task<bool> BeExistingUser(string username, CancellationToken cancellationToken)
        => _context.Users.AsNoTracking()
            .AnyAsync(u => u.Username == username, cancellationToken);
}