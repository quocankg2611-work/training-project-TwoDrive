using Microsoft.EntityFrameworkCore;
using TwoDrive.Persistence.Models;

namespace TwoDrive.Persistence.Helpers;

internal class PathMaintenanceHelper
{
    private readonly AppDbContext _dbContext;

    public PathMaintenanceHelper(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SetFolderPathAsync(FolderPersistence folder, CancellationToken cancellationToken = default)
    {
        if (folder.Id == Guid.Empty)
        {
            folder.Id = Guid.NewGuid();
        }

        if (folder.ParentFolderId is null)
        {
            folder.Path = folder.Id.ToString();
            return;
        }

        var parentPath = await _dbContext.Folders
            .Where(x => x.Id == folder.ParentFolderId.Value)
            .Select(x => x.Path)
            .SingleAsync(cancellationToken);

        folder.Path = $"{parentPath}.{folder.Id}";
    }

    public async Task SetFilePathAsync(FilePersistence file, CancellationToken cancellationToken = default)
    {
        if (file.Id == Guid.Empty)
        {
            file.Id = Guid.NewGuid();
        }

        var parentPath = await _dbContext.Folders
            .Where(x => x.Id == file.FolderId)
            .Select(x => x.Path)
            .SingleAsync(cancellationToken);

        file.Path = $"{parentPath}.{file.Id}";
    }

    public async Task MoveFolderAsync(Guid folderId, Guid? newParentFolderId, CancellationToken cancellationToken = default)
    {
        var folder = await _dbContext.Folders
            .SingleAsync(x => x.Id == folderId, cancellationToken);

        var oldPrefix = folder.Path;

        string newPrefix;
        if (newParentFolderId is null)
        {
            newPrefix = folder.Id.ToString();
        }
        else
        {
            var parentPath = await _dbContext.Folders
                .Where(x => x.Id == newParentFolderId.Value)
                .Select(x => x.Path)
                .SingleAsync(cancellationToken);

            newPrefix = $"{parentPath}.{folder.Id}";
        }

        folder.ParentFolderId = newParentFolderId;
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE Folders SET Path = REPLACE(Path, {oldPrefix}, {newPrefix}) WHERE Path LIKE {oldPrefix + "%"}",
            cancellationToken);

        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE Files SET Path = REPLACE(Path, {oldPrefix}, {newPrefix}) WHERE Path LIKE {oldPrefix + ".%"}",
            cancellationToken);
    }
}
