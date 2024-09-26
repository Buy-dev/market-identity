using Market.Identity.Application.Dtos;
using Market.Identity.Domain.Entities;

namespace Market.Identity.Application.Services;

public interface ITokenService
{
    Task<TokenResponse> GenerateTokens(User user);
}