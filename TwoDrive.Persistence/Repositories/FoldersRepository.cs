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

        existingFolder.ParentFolderId = folder.ParentFolderId;
        existingFolder.Name = folder.Name;
        existingFolder.Path = folder.Path;

        existingFolder.SetAuditFieldsOnUpdated(currentUserService);
    }

    public async Task BulkDeleteAsync(IEnumerable<Guid> ids)
    {
        var folders = await dbContext.Folders
            .Where(x => ids.Contains(x.Id))
            .ToListAsync();
        dbContext.Folders.RemoveRange(folders);
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