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
    private readonly string _secretKey = config["Jwt:Secret"]!;
    private readonly int _tokenValidityInMinutes = int.Parse(config["Jwt:TokenValidityInMinutes"]!);
    private readonly int _refreshTokenValidityInDays = int.Parse(config["Jwt:RefreshTokenValidityInDays"]!);
    
    public async Task<TokenResponse?> GenerateTokens(LoginUserDto user)
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
        
        var user = await context.Users
            .AsNoTracking()
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Where(u => u.Username == username)
            .Select(u => mapper.Map(u))
            .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
            
        if (user == null) return null;

        var storedRefreshToken = await context
            .RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Token == refreshToken 
                                      && r.UserId == user.Id
                                      && (!r.IsUsed || !r.IsRevoked),
                cancellationToken).ConfigureAwait(false);
       
        if (storedRefreshToken == null) return null;

        refreshToken = await RefreshTokenIfNeeded(storedRefreshToken, user.Id).ConfigureAwait(false);

        var newAccessToken = GenerateAccessToken(user);

        return new TokenResponse(newAccessToken, refreshToken);
    }
    
    private async Task<string> RefreshTokenIfNeeded(RefreshToken storedRefreshToken, long userId)
    {
        var timeLeftToExpiring = storedRefreshToken.Expires - DateTime.UtcNow;
        if (timeLeftToExpiring.TotalMinutes > _tokenValidityInMinutes) return storedRefreshToken.Token;

        var refreshTokenEntity = await GenerateRefreshToken(userId);
        return refreshTokenEntity.Token;
    }

    private string GenerateAccessToken(LoginUserDto user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);
        
        var claims = GetClaims(user);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claims,
            Expires = DateTime.UtcNow.AddMinutes(_tokenValidityInMinutes),
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

        await context.RefreshTokens.AddAsync(refreshTokenEntity);
        await context.SaveAsync();
        
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