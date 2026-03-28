using Microsoft.EntityFrameworkCore;
using TwoDrive.Persistence.Queries.Common;
using TwoDrive.Services.Common;

namespace TwoDrive.Persistence.Queries;

public class GetFolderByIdQuery : IQuery<GetFolderByIdQueryResult>
{
	public Guid Id { get; set; }
}

public record GetFolderByIdQueryResult : AuditQuery
{
	public Guid Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public string Path { get; set; } = string.Empty;
}

internal class GetFolderByIdQueryHandler(AppDbContext dbContext) : IQueryHandler<GetFolderByIdQuery, GetFolderByIdQueryResult>
{
	public async Task<GetFolderByIdQueryResult> Handle(GetFolderByIdQuery query)
	{
		var folder = await dbContext.Folders
			.AsNoTracking()
			.SingleOrDefaultAsync() ?? throw new KeyNotFoundException();

		var result = new GetFolderByIdQueryResult()
		{
			Id = folder.Id,
			Name = folder.Name,
			Path = folder.Path,
		};
		folder.AssignAuditQuery(result);

		return result;
	}
}