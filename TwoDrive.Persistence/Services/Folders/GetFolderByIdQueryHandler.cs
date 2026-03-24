using Microsoft.EntityFrameworkCore;
using TwoDrive.Services.Common;
using TwoDrive.Services.Folders;

namespace TwoDrive.Persistence.Services.Folders;

internal class GetFolderByIdQueryHandler : IQueryHandler<GetFolderByIdQuery, FolderDetailsDto>
{
    private readonly AppDbContext _dbContext;

    public GetFolderByIdQueryHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<FolderDetailsDto> Handle(GetFolderByIdQuery query)
    {
        var folder = await _dbContext.Folders
            .AsNoTracking()
            .Where(x => x.Id == query.FolderId)
            .Select(x => new FolderDetailsDto(
                x.Id,
                x.ParentFolderId,
                x.Name,
                x.Path,
                x.CreatedAt,
                x.UpdatedAt))
            .SingleOrDefaultAsync();

        return folder ?? throw new KeyNotFoundException($"Folder '{query.FolderId}' was not found.");
    }
}
