using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Market.Identity.Application.Dtos;
using Market.Identity.Application.Infrastructure.Mappers;
using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Market.Identity.Infrastructure.Services;

public class TokenService(
    IConfiguration config,
    UserMapper mapper,
    Repository<User> userRepository,
    Repository<RefreshToken> refreshTokenRepository)
    : ITokenService
{
    private readonly string _secretKey = config["Jwt:Secret"]!;
    private readonly int _tokenValidityInMinutes = int.Parse(config["Jwt:TokenValidityInMinutes"]!);
    private readonly int _refreshTokenValidityInDays = int.Parse(config["Jwt:RefreshTokenValidityInDays"]!);
    private readonly string _validAudience = config["Jwt:ValidAudience"]!;
    private readonly string _validIssuer = config["Jwt:ValidIssuer"]!;
    
    public async Task<TokenResponse?> GenerateTokens(UserDto user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken =  await GenerateRefreshToken(user.Id);

        return new TokenResponse(accessToken, refreshToken.Token);
    }

    public async Task<TokenResponse?> RefreshTokensAsync(string accessToken, string refreshToken, CancellationToken cancellationToken)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal?.Identity == null) return null;

        var username = principal.Identity.Name!;
        var user = await userRepository
            .GetByAndMapAsync(u => u.Username == username, mapper, cancellationToken)
            .ConfigureAwait(false);
            
        if (user == null) return null;

        var storedRefreshToken = await refreshTokenRepository
            .GetByAsync(GetValidToken(refreshToken, user.Id), cancellationToken)
            .ConfigureAwait(false);
       
        if (storedRefreshToken == null) return null;

        refreshToken = await RefreshTokenIfNeeded(storedRefreshToken, user.Id).ConfigureAwait(false);

        var newAccessToken = GenerateAccessToken(user);

        return new TokenResponse(newAccessToken, refreshToken);
    }

    private static Expression<Func<RefreshToken, bool>> GetValidToken(string refreshToken, long userId)
    {
        return r => r.Token == refreshToken 
                    && r.UserId == userId
                    && (!r.IsUsed || !r.IsRevoked);
    }

    private async Task<string> RefreshTokenIfNeeded(RefreshToken storedRefreshToken, long userId)
    {
        var timeLeftToExpiring = storedRefreshToken.Expires - DateTime.UtcNow;
        if (timeLeftToExpiring.TotalMinutes > _tokenValidityInMinutes) return storedRefreshToken.Token;

        var refreshTokenEntity = await GenerateRefreshToken(userId);
        return refreshTokenEntity.Token;
    }

    public string GenerateAccessToken(UserDto user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        
        var claims = GetClaims(user);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddMinutes(_tokenValidityInMinutes),
            Audience = _validAudience,
            Issuer = _validIssuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private ClaimsIdentity GetClaims(UserDto user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.Username),
        };

        foreach (var role in user.Roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return new ClaimsIdentity(claims);
    }

    private async Task<RefreshToken> GenerateRefreshToken(long userId)
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        var refreshToken = Convert.ToBase64String(randomNumber);
        
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(_refreshTokenValidityInDays),
            UserId = userId,
            IsUsed = false,
            IsRevoked = false
        };

        await refreshTokenRepository
            .AddAsync(refreshTokenEntity, CancellationToken.None)
            .ConfigureAwait(false);
        
        return  refreshTokenEntity;
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        };

        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

        if (securityToken is not JwtSecurityToken jwtSecurityToken 
            || jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase) == false)
        {
            throw new SecurityTokenException("Invalid token");
        }

        return principal;
    }
}