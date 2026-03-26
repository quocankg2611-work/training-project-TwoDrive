using TwoDrive.Services.__Services__;

namespace TwoDrive.Persistence.Models
{
    public abstract class AuditPersistenceBase
    {
        public string CreatedByUserId { get; set; } = string.Empty;
        public string CreatedByUserNameSnapshot { get; set; } = string.Empty;
        public string UpdatedByUserId { get; set; } = string.Empty;
        public string UpdatedByUserNameSnapshot { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public void SetAuditFieldsOnCreated(ICurrentUserService currentUserService)
        {
            CreatedByUserId = currentUserService.GetUserIdOrThrow();
            CreatedByUserNameSnapshot = currentUserService.GetUserNameOrThrow();
            UpdatedByUserId = currentUserService.GetUserIdOrThrow();
            UpdatedByUserNameSnapshot = currentUserService.GetUserNameOrThrow();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetAuditFieldsOnUpdated(ICurrentUserService currentUserService)
        {
            UpdatedByUserId = currentUserService.GetUserIdOrThrow();
            UpdatedByUserNameSnapshot = currentUserService.GetUserNameOrThrow();
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
