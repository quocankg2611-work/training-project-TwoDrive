using Microsoft.EntityFrameworkCore;
using TwoDrive.Services.Common;
using TwoDrive.Services.Folders;

namespace TwoDrive.Persistence.Services.Folders;

internal class UploadFolderCommandHandler : ICommandHandler<UploadFolderCommand>
{
    private readonly AppDbContext _dbContext;

    public UploadFolderCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(UploadFolderCommand command)
    {
        var exists = await _dbContext.Folders.AnyAsync(x => x.Id == command.FolderId);

        if (!exists)
        {
            throw new KeyNotFoundException($"Folder '{command.FolderId}' was not found.");
        }
    }
}
