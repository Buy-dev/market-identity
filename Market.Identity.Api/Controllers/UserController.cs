using Market.Identity.Application.MediatR.Commands.LoginUser;
using Market.Identity.Application.MediatR.Commands.RefreshToken;
using Market.Identity.Application.MediatR.Commands.RegisterUser;
using Microsoft.AspNetCore.Mvc;

namespace Market.Identity.Controllers;

public class UserController : BaseController
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken cancellationToken)
    {
        var result = await MediatR.Send(command, cancellationToken).ConfigureAwait(false);
        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken cancellationToken)
    {
        var result = await MediatR.Send(command, cancellationToken).ConfigureAwait(false);
        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var result = await MediatR.Send(command, cancellationToken).ConfigureAwait(false);
        return result.IsSuccess
            ? Ok(result)
            : Unauthorized(result);
    }
    
    
}