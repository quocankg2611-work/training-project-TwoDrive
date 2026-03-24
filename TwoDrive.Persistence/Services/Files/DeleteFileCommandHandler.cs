using Microsoft.EntityFrameworkCore;
using TwoDrive.Services.Common;
using TwoDrive.Services.Files;

namespace TwoDrive.Persistence.Services.Files;

internal class DeleteFileCommandHandler : ICommandHandler<DeleteFileCommand>
{
    private readonly AppDbContext _dbContext;

    public DeleteFileCommandHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Handle(DeleteFileCommand command)
    {
        var file = await _dbContext.Files.SingleOrDefaultAsync(x => x.Id == command.FileId);

        if (file is null)
        {
            throw new KeyNotFoundException($"File '{command.FileId}' was not found.");
        }

        _dbContext.Files.Remove(file);
        await _dbContext.SaveChangesAsync();
    }
}
