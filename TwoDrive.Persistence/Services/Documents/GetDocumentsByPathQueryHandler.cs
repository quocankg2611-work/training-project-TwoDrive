using Microsoft.EntityFrameworkCore;
using TwoDrive.Services.Common;
using TwoDrive.Services.Documents;

namespace TwoDrive.Persistence.Services.Documents;

internal class GetDocumentsByPathQueryHandler : IQueryHandler<GetDocumentsByPathQuery, IEnumerable<GetDocumentsQueryResultItem>>
{
    private readonly AppDbContext _dbContext;

    public GetDocumentsByPathQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<GetDocumentsQueryResultItem>> Handle(GetDocumentsByPathQuery query)
    {
        var folders = await _dbContext.Folders
            .AsNoTracking()
            .Where(x => x.Path == query.Path)
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
            .Where(x => x.Path == query.Path)
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
