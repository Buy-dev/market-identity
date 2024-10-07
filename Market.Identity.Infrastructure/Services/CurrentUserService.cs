using Market.Identity.Application.Services;
using Microsoft.AspNetCore.Http;

namespace Market.Identity.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) 
    : ICurrentUserService
{
    public string Username { get; set; } = httpContextAccessor.HttpContext!.User.Identity!.Name!;
}