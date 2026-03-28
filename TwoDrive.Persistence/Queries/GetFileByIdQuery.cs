using TwoDrive.Services.Common;
using TwoDrive.Persistence.Queries.Common;
using Microsoft.EntityFrameworkCore;

namespace TwoDrive.Persistence.Queries;

public class GetFileByIdQuery : IQuery<GetFileByIdQueryResult>
{
	public Guid Id { get; set; }
}

public record GetFileByIdQueryResult : AuditQuery
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Extension { get; set; } = string.Empty;
	public long SizeBytes { get; set; }
	public string Path { get; set; } = string.Empty;
}

internal class GetFileByIdQueryHandler(AppDbContext dbContext) : IQueryHandler<GetFileByIdQuery, GetFileByIdQueryResult>
{
	public async Task<GetFileByIdQueryResult> Handle(GetFileByIdQuery query)
	{
		var file = await dbContext.Files
			.AsNoTracking()
			.FirstOrDefaultAsync(x => x.Id == query.Id) ?? throw new KeyNotFoundException("No files found");

		var result = new GetFileByIdQueryResult()
		{
			Id = file.Id,
			Name = file.Name,
			Path = file.Path,
			Extension = file.Extension,
			SizeBytes = file.SizeBytes,
		};
		file.AssignAuditQuery(result);

		return result;
	}
}
