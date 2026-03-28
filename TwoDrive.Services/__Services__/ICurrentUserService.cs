namespace TwoDrive.Services.__Services__;

/// <summary>
/// Retrive current user from HTTP context
/// </summary>
public interface ICurrentUserService
{
    bool IsAuthenticated { get; }
    string GetUserIdOrThrow();
    string GetUserNameOrThrow();
}
