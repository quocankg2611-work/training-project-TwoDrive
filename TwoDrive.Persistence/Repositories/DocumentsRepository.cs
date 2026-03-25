using Microsoft.EntityFrameworkCore;
using TwoDrive.Services.Common;
using TwoDrive.Services.Documents;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Persistence.Repositories;

internal class DocumentsRepository : IDocumentsRepository
{
    private readonly AppDbContext _dbContext;

    public DocumentsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<GetDocumentsQueryResultItem>> GetByPathAsync(string path)
    {
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
