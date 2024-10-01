using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Market.Identity.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    protected IMediator MediatR  => (IMediator)HttpContext.RequestServices.GetService(typeof(IMediator));
}