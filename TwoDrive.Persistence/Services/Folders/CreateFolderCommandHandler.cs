using Microsoft.EntityFrameworkCore;
using TwoDrive.Persistence.Helpers;
using TwoDrive.Persistence.Models;
using TwoDrive.Services.Common;
using TwoDrive.Services.Folders;

namespace TwoDrive.Persistence.Services.Folders;

internal class CreateFolderCommandHandler : ICommandHandler<CreateFolderCommand, CreateFolderCommandResult>
{
    private readonly AppDbContext _dbContext;
    private readonly PathMaintenanceHelper _pathMaintenanceService;

    public CreateFolderCommandHandler(AppDbContext dbContext, PathMaintenanceHelper pathMaintenanceService)
    {
        _dbContext = dbContext;
        _pathMaintenanceService = pathMaintenanceService;
    }

    public async Task<CreateFolderCommandResult> Handle(CreateFolderCommand command)
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
}
