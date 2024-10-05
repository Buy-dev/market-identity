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
            .MaximumLength(40).WithMessage("Никнейм должен содержать не более 40 символов")
            .Matches("^[a-zA-Z0-9_.]*$").WithMessage("Никнейм должен содержать только буквы, цифры, символ подчеркивания, или точку")
            .MustAsync(BeUniqueUsername).WithMessage("Уже существует пользователь с данным никнеймом");

        RuleFor(x => x.FullName)
            .NotNull().WithMessage("ФИО обязательно")
            .MinimumLength(2).WithMessage("ФИО должно быть не менее 2 символов")
            .MaximumLength(100).WithMessage("ФИО должно быть не более 100 символов");

        RuleFor(x => x.Email)
            .NotNull().WithMessage("Электронная почта обязательна")
            .EmailAddress().WithMessage("Неверный формат электронной почты")
            .MaximumLength(254).WithMessage("Электронная почта должна быть не более 254 симвволов")
            .MustAsync(BeUniqueEmail).WithMessage("Уже есть пользователь с данной электронной почтой");

        RuleFor(x => x.CallSign)
            .MinimumLength(2).When(x => !string.IsNullOrEmpty(x.CallSign)).WithMessage("Позывной должен быть не менее 2 символов")
            .MaximumLength(40).When(x => !string.IsNullOrEmpty(x.CallSign)).WithMessage("Позывной должен быть не более 40 символов");

        RuleFor(x => x.Password)
            .NotNull().WithMessage("Пароль обязателен")
            .MinimumLength(4).WithMessage("Пароль должен содержать не менее 4 символов")
            .Must(p => p.Any(char.IsDigit)).WithMessage("Пароль должен содержать хотя бы одну цифру")
            .Must(p => p.Any(char.IsUpper)).WithMessage("Пароль должен содержать хотя бы одну заглавную букву")
            .Must(p => p.Any(char.IsLower)).WithMessage("Пароль должен содержать хотя бы одну строчную букву")
            .Must(p => p.Any("!@#$%^&*()-_+=<>?".Contains)).WithMessage("Пароль должен содержать хотя бы один спецсимвол");
    }

    private Task<bool> BeUniqueUsername(string username, CancellationToken cancellationToken)
    {
        return  _context.Users
                        .AsNoTracking()
                        .AllAsync(u => u.Username != username, cancellationToken);
    }

    private Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        return  _context.Users
                        .AsNoTracking()
                        .AllAsync(u => u.Email != email, cancellationToken);
    }
}