using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Market.Identity.Application.Dtos;
using Market.Identity.Application.Infrastructure.Mappers;
using Market.Identity.Application.MediatR.Commands.LoginUser;
using Market.Identity.Application.Services;
using Market.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Market.Identity.Infrastructure.Services;

public class TokenService(
    IIdentityDbContext context,
    IConfiguration config,
    LoginUserMapper mapper)
    : ITokenService
{
    private readonly string _secretKey = config["Jwt:Key"]!; // Secret for Access Token

    public async Task<TokenResponse?> GenerateTokens(LoginUserDto user)
    {
        var accessToken = GenerateAccessToken(user);

        var refreshToken = GenerateRefreshToken();
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(7),
            UserId = user.Id,
            IsUsed = false,
            IsRevoked = false
        };

        await context.RefreshTokens.AddAsync(refreshTokenEntity);
        var saveResult = await context.SaveAsync().ConfigureAwait(false);

        return saveResult
            ? new TokenResponse(accessToken, refreshToken)
            : null;
    }

    public async Task<TokenResponse?> RefreshTokensAsync(string accessToken, string refreshToken, CancellationToken cancellationToken)
    {
        var principal = GetPrincipalFromExpiredToken(accessToken);
        if (principal?.Identity == null) return null;

        var username = principal.Identity.Name!;
        
        var user = await context.Users
            .AsNoTracking()
            .Where(u => u.Username == username)
            .Select(u => mapper.Map(u))
            .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            
        if (user == null) return null;

        var storedRefreshToken = await context
            .RefreshTokens
            .FirstOrDefaultAsync(r => IsValidRefreshToken(refreshToken, user.Id),
                cancellationToken).ConfigureAwait(false);
        if (storedRefreshToken == null)
            return null;

        storedRefreshToken.IsUsed = true;
        context.RefreshTokens.Update(storedRefreshToken);
        var saveResult = await context.SaveAsync(cancellationToken).ConfigureAwait(false);

        return saveResult
            ? await GenerateTokens(user)
            : null;
    }
    
    private bool IsValidRefreshToken(string refreshToken, long userId)
    {
        return context.RefreshTokens.Any(r => r.Token == refreshToken && r.UserId == userId
            && (!r.IsUsed || !r.IsRevoked || r.Expires < DateTime.Now));
    }

    private string GenerateAccessToken(LoginUserDto user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        
        var claims = GetClaims(user);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private ClaimsIdentity GetClaims(LoginUserDto user)
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

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
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