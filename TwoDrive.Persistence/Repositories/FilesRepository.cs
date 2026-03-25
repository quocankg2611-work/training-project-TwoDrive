using Microsoft.EntityFrameworkCore;
using TwoDrive.Core.Models;
using TwoDrive.Persistence.Models;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Persistence.Repositories;

internal class FilesRepository(
    AppDbContext dbContext) : IFilesRepository
{
    public Task CreateAsync(FileModel file)
    {
        var filePersistence = ToPersistence(file);
        return dbContext.Files.AddAsync(filePersistence).AsTask();
    }

    public async Task UpdateAsync(FileModel file)
    {
        var existingFile = await dbContext.Files.SingleOrDefaultAsync(x => x.Id == file.Id);

        if (existingFile is null)
        {
            throw new KeyNotFoundException($"File '{file.Id}' was not found.");
        }

        existingFile.FolderId = file.FolderId ?? throw new ArgumentException("FolderId is required.", nameof(file));
        existingFile.OwnerId = file.OwnerId;
        existingFile.Name = file.Name;
        existingFile.Extension = file.Extension;
        existingFile.Path = file.Path;
        existingFile.MimeType = file.MimeType;
        existingFile.SizeBytes = file.SizeBytes;
        existingFile.StorageKey = file.StorageKey;
        existingFile.Checksum = file.Checksum;
    }

    public async Task DeleteAsync(Guid fileId)
    {
        var file = await dbContext.Files.SingleOrDefaultAsync(x => x.Id == fileId);

        if (file is null)
        {
            throw new KeyNotFoundException($"File '{fileId}' was not found.");
        }

        dbContext.Files.Remove(file);
        await dbContext.SaveChangesAsync();
    }

    private static FilePersistence ToPersistence(FileModel file)
    {
        return new FilePersistence
        {
            Id = file.Id,
            FolderId = file.FolderId,
            OwnerId = file.OwnerId,
            Name = file.Name,
            Extension = file.Extension,
            Path = file.Path,
            MimeType = file.MimeType,
            SizeBytes = file.SizeBytes,
            StorageKey = file.StorageKey,
            Checksum = file.Checksum,
        };
    }
}
