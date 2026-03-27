using Microsoft.EntityFrameworkCore;
using TwoDrive.Core;
using TwoDrive.Core.Helper;
using TwoDrive.Core.Models;
using TwoDrive.Persistence.Models;
using TwoDrive.Services.__Persistence__;
using TwoDrive.Services.__Services__;

namespace TwoDrive.Persistence.Repositories;

internal class FoldersRepository(AppDbContext dbContext, ICurrentUserService currentUserService) : IFoldersRepository
{
    public async Task<FolderModel?> GetByPathAsync(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || path == PathUtils.ROOT_PATH)
        {
            return null;
        }

        var normalizedPath = path.TrimEnd('/');
        var parentPath = PathUtils.GetParentPath(normalizedPath);
        var folderName = PathUtils.GetParentPathName(normalizedPath);

        var folder = await dbContext.Folders.FirstOrDefaultAsync(x => x.Path == parentPath && x.Name == folderName);

        return folder is null ? null : ToModel(folder);
    }

    public async Task<FolderModel?> GetByIdAsync(Guid folderId)
    {
        var folder = await dbContext.Folders.SingleOrDefaultAsync(x => x.Id == folderId);
        return folder is null ? null : ToModel(folder);
    }

    public async Task<IEnumerable<FolderModel>> GetByIdsAsync(IEnumerable<Guid> folderIds)
    {
        var folders = await dbContext.Folders
            .Where(x => folderIds.Contains(x.Id))
            .ToListAsync();
        return folders.Select(ToModel);
    }

    public async Task<Guid> CreateAsync(FolderModel folder)
    {
        var folderPersistence = ToPersistence(folder);
        folderPersistence.SetAuditFieldsOnCreated(currentUserService);

        await dbContext.Folders.AddAsync(folderPersistence);
        return folderPersistence.Id;
    }

    public async Task BulkCreateAsync(IEnumerable<FolderModel> folders)
    {
        var folderPersistences = folders.Select(folder =>
        {
            var persistence = ToPersistence(folder);
            persistence.SetAuditFieldsOnCreated(currentUserService);
            return persistence;
        });
        await dbContext.Folders.AddRangeAsync(folderPersistences);
    }

    public async Task UpdateAsync(FolderModel folder)
    {
        var existingFolder = await dbContext.Folders
            .SingleOrDefaultAsync(x => x.Id == folder.Id);

        if (existingFolder is null)
        {
            throw new KeyNotFoundException($"Folder '{folder.Id}' was not found.");
        }

        var subFolderPath = existingFolder.Path + existingFolder.Name + '/';

        existingFolder.ParentFolderId = folder.ParentFolderId;
        existingFolder.Name = folder.Name;
        existingFolder.Path = folder.Path;

        existingFolder.SetAuditFieldsOnUpdated(currentUserService);
    }

    /// <summary>
    /// We need to recursively update the path of all child folders when the folder name is updated, to keep the path consistent with the folder name.
    /// </summary>
    /// <param name="folderId"></param>
    /// <param name="newName"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException"></exception>
    public async Task UpdateFolderNameAsync(Guid folderId, string newName)
    {
        var existingFolder = await dbContext.Folders
            .SingleOrDefaultAsync(x => x.Id == folderId) ?? throw new KeyNotFoundException($"Folder '{folderId}' was not found.");

        var orgSubFolderPath = existingFolder.PathForChildren;

        // Update properties
        existingFolder.Name = newName;
        existingFolder.SetAuditFieldsOnUpdated(currentUserService);
        await dbContext.SaveChangesAsync();

        var subFolderPath = existingFolder.PathForChildren;

        // Scan all child folders and update their path in batch
        await dbContext.Folders
            .Where(x => x.Path.StartsWith(orgSubFolderPath))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Path, x => subFolderPath + x.Path.Substring(orgSubFolderPath.Length))
                );

        // Scan all child files and update their path in batch
        await dbContext.Files
            .Where(x => x.Path.StartsWith(orgSubFolderPath))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.Path, x => subFolderPath + x.Path.Substring(orgSubFolderPath.Length))
                );
    }

    /// <summary>
    /// SQL do not allow self-referencing table to be deleted in cascade
    /// We need to recursively find all child folders and delete them together with the parent folder in one transaction to avoid the cascade delete issue.
    /// </summary>
    /// <param name="ids"></param>
    /// <returns></returns>
    public async Task BulkDeleteAsync(IEnumerable<Guid> ids)
    {
        if (ids.Any() == false) return;

        var subFolderQueryPaths = await dbContext.Folders
            .Where(x => ids.Contains(x.Id))
            .Select(x => x.PathForChildren)
            .ToListAsync();

        var lambda = RepositoryUtils.BuildFolderByPathExpression(subFolderQueryPaths) ?? throw new InvalidOperationException("Failed to build folder paths expression.");

        // Delete sub-folders
        await dbContext.Folders
            .Where(lambda)
            .ExecuteDeleteAsync();

        // Delete folders
        await dbContext.Folders.Where(x => ids.Contains(x.Id)).ExecuteDeleteAsync();
    }

    private static FolderModel ToModel(FolderPersistence folder)
    {
        return new FolderModel
        {
            Id = folder.Id,
            ParentFolderId = folder.ParentFolderId,
            Name = folder.Name,
            Path = folder.Path,
        };
    }
    private static FolderPersistence ToPersistence(FolderModel folder)
    {
        return new FolderPersistence
        {
            Id = folder.Id,
            ParentFolderId = folder.ParentFolderId,
            Name = folder.Name,
            Path = folder.Path,
        };
    }
}