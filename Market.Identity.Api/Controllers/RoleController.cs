using System.Threading;
using System.Threading.Tasks;
using Market.Identity.Application.MediatR.Commands.AssignRole.AssignRoleToMe;
using Market.Identity.Application.MediatR.Queries.GetRolesTree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Market.Identity.Controllers;

public class RoleController : BaseController
{
    [Authorize]
    [HttpGet("roles-tree")]
    public async Task<IActionResult> GetRolesTree(CancellationToken cancellationToken)
    {
        var result = await MediatR.Send(new GetRolesTreeQuery(), cancellationToken)
            .ConfigureAwait(false);
        return Ok(result);
    }
    
    [Authorize]
    [HttpPut("assign-role-to-me")]
    public async Task<IActionResult> AssignRoleToMe([FromBody] AssignRoleToMeCommand command, CancellationToken cancellationToken)
    {
        var result = await MediatR.Send(command, cancellationToken).ConfigureAwait(false);
        return result.IsSuccess
            ? Ok(result)
            : BadRequest(result);
    }
}