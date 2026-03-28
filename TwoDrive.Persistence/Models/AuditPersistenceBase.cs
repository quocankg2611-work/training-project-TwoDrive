using TwoDrive.Persistence.Queries.Common;
using TwoDrive.Services.__Services__;

namespace TwoDrive.Persistence.Models
{
	public abstract class AuditPersistenceBase
	{
		public string CreatedByUserId { get; set; } = string.Empty;
		public string CreatedByUserNameSnapshot { get; set; } = string.Empty;
		public string UpdatedByUserId { get; set; } = string.Empty;
		public string UpdatedByUserNameSnapshot { get; set; } = string.Empty;
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset UpdatedAt { get; set; }

		public void SetAuditFieldsOnCreated(ICurrentUserService currentUserService)
		{
			CreatedByUserId = currentUserService.GetUserIdOrThrow();
			CreatedByUserNameSnapshot = currentUserService.GetUserNameOrThrow();
			UpdatedByUserId = currentUserService.GetUserIdOrThrow();
			UpdatedByUserNameSnapshot = currentUserService.GetUserNameOrThrow();
			CreatedAt = DateTimeOffset.UtcNow;
			UpdatedAt = DateTimeOffset.UtcNow;
		}

		public void SetAuditFieldsOnUpdated(ICurrentUserService currentUserService)
		{
			UpdatedByUserId = currentUserService.GetUserIdOrThrow();
			UpdatedByUserNameSnapshot = currentUserService.GetUserNameOrThrow();
			UpdatedAt = DateTimeOffset.UtcNow;
		}

		public void AssignAuditQuery<T>(T auditQueryBase) where T : AuditQuery
		{
			auditQueryBase.CreatedAt = CreatedAt;
			auditQueryBase.UpdatedAt = UpdatedAt;
			auditQueryBase.CreatedByName = CreatedByUserNameSnapshot;
			auditQueryBase.UpdatedByName = UpdatedByUserNameSnapshot;
		}
	}
}
