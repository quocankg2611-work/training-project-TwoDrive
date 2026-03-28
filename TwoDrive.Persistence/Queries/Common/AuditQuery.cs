namespace TwoDrive.Persistence.Queries.Common;

public abstract record AuditQuery
{
	public DateTimeOffset CreatedAt { get; set; }
	public string CreatedByName { get; set; } = string.Empty;
	public DateTimeOffset UpdatedAt { get; set; }
	public string UpdatedByName { get; set; } = string.Empty;
}

