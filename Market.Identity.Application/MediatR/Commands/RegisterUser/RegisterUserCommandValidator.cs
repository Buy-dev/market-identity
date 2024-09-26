using FluentValidation;
using Market.Identity.Application.Services;
using Microsoft.EntityFrameworkCore;

namespace Market.Identity.Application.MediatR.Commands.RegisterUser;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    private readonly IIdentityDbContext _context;

    public RegisterUserCommandValidator(IIdentityDbContext context)
    {
        _context = context;

        RuleFor(x => x.Username)
            .NotNull().WithMessage("Никнейм обязателен")
            .MinimumLength(2).WithMessage("Никнейм должен содержать не менее 2 символов")
            .MustAsync(BeUniqueUsername).WithMessage("Уже существует пользователь с данным никнеймом");

        RuleFor(x => x.FullName)
            .MinimumLength(2).WithMessage("ФИО должно быть не менее 2 символов");

        RuleFor(x => x.Email)
            .NotNull().WithMessage("Электронная почта обязательна")
            .EmailAddress().WithMessage("Неверный формат электронной почты")
            .MustAsync(BeUniqueEmail).WithMessage("Уже есть пользователь с данной электронной почтой");

        RuleFor(x => x.CallSign)
            .MinimumLength(2).When(x => !string.IsNullOrEmpty(x.CallSign))
            .WithMessage("Позывной должен быть не менее 2 символов");
    }

    private Task<bool> BeUniqueUsername(string username, CancellationToken cancellationToken)
    {
        return  _context.Users.AllAsync(u => u.Username != username, cancellationToken);
    }

    private Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return  _context.Users.AllAsync(u => u.Email != email, cancellationToken);
    }
}