using Microsoft.EntityFrameworkCore;
using TwoDrive.Persistence.Helpers;
using TwoDrive.Persistence.Models;
using TwoDrive.Services.Common;
using TwoDrive.Services.Files;

namespace TwoDrive.Persistence.Services.Files;

internal class UploadFileCommandHandler : ICommandHandler<UploadFileCommand, UploadFileCommandResult>
{
    private readonly AppDbContext _dbContext;
    private readonly PathMaintenanceHelper _pathMaintenanceHelper;

    public UploadFileCommandHandler(AppDbContext dbContext, PathMaintenanceHelper pathMaintenanceService)
    {
        _dbContext = dbContext;
        _pathMaintenanceHelper = pathMaintenanceService;
    }

    public async Task<UploadFileCommandResult> Handle(UploadFileCommand command)
    {
        var folderOwnerId = await _dbContext.Folders
            .Where(x => x.Id == command.FolderId)
            .Select(x => (Guid?)x.OwnerId)
            .SingleOrDefaultAsync();

        if (folderOwnerId is null)
        {
            throw new KeyNotFoundException($"Folder '{command.FolderId}' was not found.");
        }

        var storageKey = $"{command.FolderId:N}/{Guid.NewGuid():N}";

        var file = new FileModel
        {
            FolderId = command.FolderId,
            OwnerId = folderOwnerId.Value,
            Name = command.FileName,
            MimeType = command.MimeType,
            SizeBytes = command.FileSizeBytes,
            StorageKey = storageKey,
            Checksum = string.Empty
        };

        await _pathMaintenanceHelper.SetFilePathAsync(file);

        _dbContext.Files.Add(file);
        await _dbContext.SaveChangesAsync();

        return new UploadFileCommandResult(file.Id, file.StorageKey);
    }
}
