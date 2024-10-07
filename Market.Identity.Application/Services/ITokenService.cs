using Market.Identity.Application.Dtos;

namespace Market.Identity.Application.Services;

public interface ITokenService
{
    Task<TokenResponse?> GenerateTokens(UserDto user);

    Task<TokenResponse?> RefreshTokensAsync(string accessToken, string refreshToken,
        CancellationToken cancellationToken);

    string GenerateAccessToken(UserDto user);
}