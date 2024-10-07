using Market.Identity.Application.Dtos;
using Market.Identity.Application.Infrastructure.Mappers;
using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Market.Identity.Application.MediatR.Commands.LoginUser;

public record LoginUserCommand : IRequest<Result<TokenResponse>>
{
    public string Username { get; init; }
    public string Password { get; init; }
}

public class LoginUserCommandHandler(
    IIdentityDbContext context,
    IPasswordHasher<User> passwordHasher,
    ITokenService tokenService,
    UserMapper mapper,
    IRepository<User> userRepository)
    : IRequestHandler<LoginUserCommand, Result<TokenResponse>>
{
    public async Task<Result<TokenResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository
            .GetByAndMapAsync(u => u.Username == request.Username, mapper, cancellationToken)
            .ConfigureAwait(false);
        
        var hashingResult = passwordHasher.VerifyHashedPassword(null, user!.PasswordHash, request.Password);
        if (hashingResult == PasswordVerificationResult.Failed)
            return Result<TokenResponse>.Failure("Неверный пароль");

        var tokens = await tokenService.GenerateTokens(user).ConfigureAwait(false);

        return tokens != null
            ? Result<TokenResponse>.Success(tokens)
            : Result<TokenResponse>.Failure("Ошибка при генерации токенов");
    }
}