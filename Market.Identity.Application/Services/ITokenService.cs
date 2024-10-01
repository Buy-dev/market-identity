using Market.Identity.Application.Dtos;
using Market.Identity.Application.MediatR.Commands.LoginUser;
using Market.Identity.Domain.Entities;

namespace Market.Identity.Application.Services;

public interface ITokenService
{
    Task<TokenResponse?> GenerateTokens(LoginUserDto user);

    Task<TokenResponse?> RefreshTokensAsync(string accessToken, string refreshToken,
        CancellationToken cancellationToken);
}