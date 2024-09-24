using Market.Identity.Application.Dtos;
using Market.Identity.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Market.Identity.Controllers;

public class UserController(IUserService userService, 
                            ITokenService tokenService) : BaseController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        var user = await userService.AuthenticateUserAsync(loginDto);
        if (user == null) return Unauthorized();

        var tokens = await tokenService.GenerateTokensAsync(user);
        return Ok(tokens);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var newTokens = await tokenService.RefreshTokensAsync(request.AccessToken, request.RefreshToken);
        if (newTokens == null) return Unauthorized();

        return Ok(newTokens);
    }
}