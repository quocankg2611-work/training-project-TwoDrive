using Microsoft.EntityFrameworkCore;
using TwoDrive.Core.Models;
using TwoDrive.Persistence.Models;
using TwoDrive.Services.__Persistence__;
using TwoDrive.Services.__Services__;

namespace TwoDrive.Persistence.Repositories;

internal class FilesRepository(AppDbContext dbContext, ICurrentUserService currentUserService) : IFilesRepository
{
    public async Task<FileModel?> GetByIdAsync(Guid id)
    {
        var file = await dbContext.Files.SingleOrDefaultAsync(x => x.Id == id);
        return file != null ? ToModel(file) : null;
    }

    public async Task<IEnumerable<FileModel>> GetByIdsAsync(IEnumerable<Guid> ids)
    {
        var files = await dbContext.Files
            .Where(x => ids.Contains(x.Id))
            .Select(x => ToModel(x))
            .ToListAsync();
        return files;
    }

    /// <summary>
    /// Get all files that are in folder list or any of its subfolders (recursively).
    /// </summary>
    /// <param name="folderIds"></param>
    /// <returns></returns>
    public async Task<IEnumerable<FileModel>> QueryDeepChildrenOfFolders(IEnumerable<FolderModel> folderModels)
    {
        // We have "Path" field on both Folder and File, which contains the full path from the root
        // This allows us to efficiently query all files that are in the specified folder or any of its subfolders by using a "StartsWith" query on the file's Path.
        var files = await dbContext.Files
            .Where(file => folderModels.Any(folder => file.Path.StartsWith(folder.Path)))
            .Select(file => ToModel(file))
            .ToListAsync();
        return files;
    }

    public Task CreateAsync(FileModel file)
    {
        var filePersistence = ToPersistence(file);
        filePersistence.SetAuditFieldsOnCreated(currentUserService);
        return dbContext.Files.AddAsync(filePersistence).AsTask();
    }

    public async Task UpdateAsync(FileModel file)
    {
        var existingFile = await dbContext.Files.SingleOrDefaultAsync(x => x.Id == file.Id) ?? throw new KeyNotFoundException($"File '{file.Id}' was not found.");

        existingFile.FolderId = file.FolderId ?? throw new ArgumentException("FolderId is required.", nameof(file));
        existingFile.Name = file.Name;
        existingFile.Extension = file.Extension;
        existingFile.Path = file.Path;
        existingFile.MimeType = file.MimeType;
        existingFile.SizeBytes = file.SizeBytes;
        existingFile.StorageKey = file.StorageKey;
        existingFile.Checksum = file.Checksum;

        existingFile.SetAuditFieldsOnUpdated(currentUserService);

        await dbContext.SaveChangesAsync();
    }

    public async Task BulkDeleteAsync(IEnumerable<Guid> fileIds)
    {
        var files = await dbContext.Files.Where(x => fileIds.Contains(x.Id)).ToListAsync();
        dbContext.Files.RemoveRange(files);
        await dbContext.SaveChangesAsync();
    }

    private static FilePersistence ToPersistence(FileModel file)
    {
        return new FilePersistence
        {
            Id = file.Id,
            FolderId = file.FolderId,
            Name = file.Name,
            Extension = file.Extension,
            Path = file.Path,
            MimeType = file.MimeType,
            SizeBytes = file.SizeBytes,
            StorageKey = file.StorageKey,
            Checksum = file.Checksum,
        };
    }

    private static FileModel ToModel(FilePersistence file)
    {
        return new FileModel
        {
            Id = file.Id,
            FolderId = file.FolderId,
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
