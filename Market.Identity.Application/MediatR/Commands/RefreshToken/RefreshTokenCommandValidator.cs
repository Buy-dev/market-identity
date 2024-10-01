using FluentValidation;

namespace Market.Identity.Application.MediatR.Commands.RefreshToken;

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(v => v.RefreshToken)
            .NotNull().WithMessage("Refresh token обязателен")
            .MinimumLength(32).WithMessage("Refresh token должен содержать не менее 32 символов");

        RuleFor(v => v.AccessToken)
            .NotNull().WithMessage("Access token обязателен");
    }
}