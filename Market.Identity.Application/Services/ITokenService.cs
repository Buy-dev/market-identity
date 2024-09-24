using Market.Identity.Application.Dtos;
using Market.Identity.Domain.Entities;

namespace Market.Identity.Application.Services;

public interface ITokenService
{
    Task<AuthResponseDto> GenerateTokensAsync(User user);
    Task<AuthResponseDto?> RefreshTokensAsync(string accessToken, string refreshToken);
}