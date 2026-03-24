using Microsoft.EntityFrameworkCore;
using TwoDrive.Persistence.Helpers;
using TwoDrive.Persistence.Models;
using TwoDrive.Services.Common;
using TwoDrive.Services.Files;

namespace TwoDrive.Persistence.Services.Files;

internal class CreateFileCommandHandler : ICommandHandler<CreateFileCommand, CreateFileCommandResult>
{
    private readonly AppDbContext _dbContext;
    private readonly PathMaintenanceHelper _pathMaintenanceService;

    public CreateFileCommandHandler(AppDbContext dbContext, PathMaintenanceHelper pathMaintenanceService)
    {
        _dbContext = dbContext;
        _pathMaintenanceService = pathMaintenanceService;
    }

    public async Task<CreateFileCommandResult> Handle(CreateFileCommand command)
    {
        var folderOwnerId = await _dbContext.Folders
            .Where(x => x.Id == command.FolderId)
            .Select(x => (Guid?)x.OwnerId)
            .SingleOrDefaultAsync();

        if (folderOwnerId is null)
        {
            throw new KeyNotFoundException($"Folder '{command.FolderId}' was not found.");
        }

        var file = new FileModel
        {
            FolderId = command.FolderId,
            OwnerId = folderOwnerId.Value,
            Name = command.Name,
            MimeType = command.MimeType,
            SizeBytes = command.SizeBytes,
            StorageKey = command.StorageKey,
            Checksum = command.Checksum
        };

        await _pathMaintenanceService.SetFilePathAsync(file);

        _dbContext.Files.Add(file);
        await _dbContext.SaveChangesAsync();

        return new CreateFileCommandResult(file.Id);
    }
}
