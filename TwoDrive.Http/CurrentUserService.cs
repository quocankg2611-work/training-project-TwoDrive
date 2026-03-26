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
        return httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == HttpConstants.Claim.UserId)?.Value ?? throw new InvalidOperationException("User ID not found.");
    }

    public string GetUserNameOrThrow()
    {
        return httpContextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == HttpConstants.Claim.Name)?.Value ?? throw new InvalidOperationException("User name not found.");
    }
}
