using Microsoft.EntityFrameworkCore;
using TwoDrive.Persistence.Helpers;
using TwoDrive.Services.Common;
using TwoDrive.Services.Folders;

namespace TwoDrive.Persistence.Services.Folders;

internal class UpdateFolderCommandHandler : ICommandHandler<UpdateFolderCommand>
{
    private readonly AppDbContext _dbContext;
    private readonly PathMaintenanceHelper _pathMaintenanceService;

    public UpdateFolderCommandHandler(AppDbContext dbContext, PathMaintenanceHelper pathMaintenanceService)
    {
        _dbContext = dbContext;
        _pathMaintenanceService = pathMaintenanceService;
    }

    public async Task Handle(UpdateFolderCommand command)
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
}
