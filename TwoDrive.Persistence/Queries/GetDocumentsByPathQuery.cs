using Microsoft.EntityFrameworkCore;
using TwoDrive.Core.Models;
using TwoDrive.Services.Common;

namespace TwoDrive.Persistence.Queries;

public class GetDocumentsByPathQuery : IQuery<IEnumerable<GetDocumentsQueryResultItem>>
{
    public string Path { get; set; } = null!;
}

public record GetDocumentsQueryResultItem(
    Guid Id,
    string Name,
    string Path,
    string DocumentType,
    string? FileType,
    string ModifiedBy,
    DateTime CreatedAt,
    DateTime UpdatedAt
    );

internal class GetDocumentsByPathQueryHandler(AppDbContext _dbContext) : IQueryHandler<GetDocumentsByPathQuery, IEnumerable<GetDocumentsQueryResultItem>>
{
    public async Task<IEnumerable<GetDocumentsQueryResultItem>> Handle(GetDocumentsByPathQuery query)
    {
        var path = query.Path;
        var folders = await _dbContext.Folders
            .AsNoTracking()
            .Where(x => x.Path == path)
            .Select(x => new GetDocumentsQueryResultItem(
                x.Id,
                x.Name,
                x.Path,
                DocumentTypeEnum.Folder.ToDocumentTypeString(),
                null,
                x.OwnerId.ToString(),
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync();

        var files = await _dbContext.Files
            .AsNoTracking()
            .Where(x => x.Path == path)
            .Select(x => new GetDocumentsQueryResultItem(
                x.Id,
                x.Name,
                x.Path,
                DocumentTypeEnum.File.ToDocumentTypeString(),
                x.MimeType,
                x.OwnerId.ToString(),
                x.CreatedAt,
                x.UpdatedAt))
            .ToListAsync();

        return folders.Concat(files);
    }
}
