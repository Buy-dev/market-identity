using Market.Identity.Application.Dtos;
using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Market.Identity.Application.MediatR.Commands.LoginUser;

public class LoginUserCommand : IRequest<Result<TokenResponse>>
{
    public string Email { get; set; }
    public string Password { get; set; }
}

public class LoginUserCommandHandler(
    IIdentityDbContext context,
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService)
    : IRequestHandler<LoginUserCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context
                         .Users
                         .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);
        if (user == null)
            return Result<TokenResponse>.Failure("Не существует пользователя с таким email");
        
        var hashingResult = passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (hashingResult == PasswordVerificationResult.Failed)
            return Result<TokenResponse>.Failure("Неверный пароль");

        // Generate access and refresh tokens
        var tokens = tokenService.GenerateTokens(user);

        return tokens;
    }
}