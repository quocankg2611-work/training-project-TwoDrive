using Microsoft.EntityFrameworkCore;
using TwoDrive.Persistence.Models;
using TwoDrive.Services.Common;
using TwoDrive.Services.Folders;

namespace TwoDrive.Persistence.Services.Folders;

internal class DeleteFolderCommandHandler : ICommandHandler<DeleteFolderCommand>
{
    private readonly AppDbContext _dbContext;

    public DeleteFolderCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DeleteFolderCommand command)
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
}
