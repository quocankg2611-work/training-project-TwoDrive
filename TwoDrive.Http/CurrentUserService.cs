using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TwoDrive.Services.__Services__;

namespace TwoDrive.Http;

internal class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string GetUserIdOrThrow()
    {
        return httpContextAccessor.HttpContext?.User?.FindFirst("oid")?.Value ?? throw new InvalidOperationException("User ID not found.");
    }

    public string GetUserNameOrThrow()
    {
        return httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? throw new InvalidOperationException("User name not found.");
    }
}
