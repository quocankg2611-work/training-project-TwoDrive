using Microsoft.EntityFrameworkCore;
using TwoDrive.Persistence.Helpers;
using TwoDrive.Persistence.Models;
using TwoDrive.Services.Folders;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Persistence.Repositories;

internal class FoldersRepository : IFoldersRepository
{
    private readonly AppDbContext _dbContext;
    private readonly PathMaintenanceHelper _pathMaintenanceService;

    public FoldersRepository(AppDbContext dbContext, PathMaintenanceHelper pathMaintenanceService)
    {
        _dbContext = dbContext;
        _pathMaintenanceService = pathMaintenanceService;
    }

    public async Task<CreateFolderCommandResult> CreateAsync(CreateFolderCommand command)
    {
        var ownerId = Guid.Empty;

        if (command.ParentFolderId is not null)
        {
            ownerId = await _dbContext.Folders
                .Where(x => x.Id == command.ParentFolderId.Value)
                .Select(x => (Guid?)x.OwnerId)
                .SingleOrDefaultAsync()
                ?? throw new KeyNotFoundException($"Parent folder '{command.ParentFolderId}' was not found.");
        }

        var folder = new FolderModel
        {
            ParentFolderId = command.ParentFolderId,
            OwnerId = ownerId,
            Name = command.Name
        };

        await _pathMaintenanceService.SetFolderPathAsync(folder);

        _dbContext.Folders.Add(folder);
        await _dbContext.SaveChangesAsync();

        return new CreateFolderCommandResult(folder.Id);
    }

    public async Task<FolderDetailsDto?> GetByIdAsync(Guid folderId)
    {
        return await _dbContext.Folders
            .AsNoTracking()
            .Where(x => x.Id == folderId)
            .Select(x => new FolderDetailsDto(
                x.Id,
                x.ParentFolderId,
                x.Name,
                x.Path,
                x.CreatedAt,
                x.UpdatedAt))
            .SingleOrDefaultAsync();
    }

    public async Task UpdateAsync(UpdateFolderCommand command)
    {
        var folder = await _dbContext.Folders.SingleOrDefaultAsync(x => x.Id == command.FolderId);

        if (folder is null)
        {
            throw new KeyNotFoundException($"Folder '{command.FolderId}' was not found.");
        }

        if (!string.IsNullOrWhiteSpace(command.NewName))
        {
            folder.Name = command.NewName;
        }

        if (command.NewParentFolderId is null || command.NewParentFolderId == folder.ParentFolderId)
        {
            await _dbContext.SaveChangesAsync();
            return;
        }

        if (command.NewParentFolderId == command.FolderId)
        {
            throw new InvalidOperationException("A folder cannot be moved under itself.");
        }

        var newParentPath = await _dbContext.Folders
            .Where(x => x.Id == command.NewParentFolderId.Value)
            .Select(x => x.Path)
            .SingleOrDefaultAsync();

        if (newParentPath is null)
        {
            throw new KeyNotFoundException($"Target parent folder '{command.NewParentFolderId}' was not found.");
        }

        if (newParentPath == folder.Path || newParentPath.StartsWith(folder.Path + "."))
        {
            throw new InvalidOperationException("A folder cannot be moved into one of its descendants.");
        }

        await _pathMaintenanceService.MoveFolderAsync(folder.Id, command.NewParentFolderId);
    }

    public async Task DeleteAsync(DeleteFolderCommand command)
    {
        var folder = await _dbContext.Folders.SingleOrDefaultAsync(x => x.Id == command.FolderId);

        if (folder is null)
        {
            throw new KeyNotFoundException($"Folder '{command.FolderId}' was not found.");
        }

        if (command.Recursive)
        {
            var folderPrefix = folder.Path;

            var foldersToDelete = await _dbContext.Folders
                .Where(x => x.Path == folderPrefix || x.Path.StartsWith(folderPrefix + "."))
                .OrderByDescending(x => x.Path.Length)
                .ToListAsync();

            var folderIds = foldersToDelete.Select(x => x.Id).ToList();

            var filesToDelete = await _dbContext.Files
                .Where(x => folderIds.Contains(x.FolderId))
                .ToListAsync();

            _dbContext.Files.RemoveRange(filesToDelete);
            _dbContext.Folders.RemoveRange(foldersToDelete);
            await _dbContext.SaveChangesAsync();
            return;
        }

        var hasChildFolders = await _dbContext.Folders.AnyAsync(x => x.ParentFolderId == command.FolderId);
        var hasFiles = await _dbContext.Files.AnyAsync(x => x.FolderId == command.FolderId);

        if (hasChildFolders || hasFiles)
        {
            throw new InvalidOperationException("Folder is not empty. Use recursive delete to remove descendants.");
        }

        _dbContext.Folders.Remove(folder);
        await _dbContext.SaveChangesAsync();
    }

    public async Task EnsureExistsAsync(Guid folderId)
    {
        var exists = await _dbContext.Folders.AnyAsync(x => x.Id == folderId);

        if (!exists)
        {
            throw new KeyNotFoundException($"Folder '{folderId}' was not found.");
        }
    }
}
