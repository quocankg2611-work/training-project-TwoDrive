using Microsoft.EntityFrameworkCore;
using TwoDrive.Core;
using TwoDrive.Core.Helper;
using TwoDrive.Persistence.Helpers;
using TwoDrive.Persistence.Models;
using TwoDrive.Services.Files;
using TwoDrive.Services.__Persistence__;
using TwoDrive.Services.__Services__;

namespace TwoDrive.Persistence.Repositories;

internal class FilesRepository : IFilesRepository
{
    private readonly AppDbContext _dbContext;
    private readonly PathMaintenanceHelper _pathMaintenanceHelper;
    private readonly IFileStorageService _fileStorageService;

    public FilesRepository(
        AppDbContext dbContext,
        PathMaintenanceHelper pathMaintenanceService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _pathMaintenanceHelper = pathMaintenanceService;
        _fileStorageService = fileStorageService;
    }

    public async Task<FileDetailsDto?> GetByIdAsync(Guid fileId)
    {
        return await _dbContext.Files
            .AsNoTracking()
            .Where(x => x.Id == fileId)
            .Select(x => new FileDetailsDto(
                x.Id,
                x.FolderId,
                x.Name,
                x.MimeType,
                x.SizeBytes,
                x.StorageKey,
                x.Checksum,
                x.CreatedAt,
                x.UpdatedAt))
            .SingleOrDefaultAsync();
    }

    public async Task DeleteAsync(Guid fileId)
    {
        var file = await _dbContext.Files.SingleOrDefaultAsync(x => x.Id == fileId);

        if (file is null)
        {
            throw new KeyNotFoundException($"File '{fileId}' was not found.");
        }

        _dbContext.Files.Remove(file);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<UploadFileCommandResult> UploadAsync(UploadFileCommand command)
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

    public async Task UploadFilesAsync(UploadFilesCommand command)
    {
        var persistMetadataTasks = command.FilesData.Select(fileData => PersistSingleFileMetadataAsync(command.BasePath, fileData));
        var uploadFileTask = UploadFileAsync(command);
        await Task.WhenAll([.. persistMetadataTasks, uploadFileTask]);
    }

    private async Task PersistSingleFileMetadataAsync(string basePath, UploadFilesCommand.Item fileData)
    {
        var folderModels = await GetFoldersToCreateAsync(basePath, fileData.Path);
        var fileModel = new FileModel
        {
            Id = Guid.NewGuid(),
            Name = fileData.FileName,
            Extension = fileData.FileExtension,
            MimeType = fileData.MimeType,
            SizeBytes = fileData.FileSizeBytes,
            FolderId = folderModels.Last().Id,
            Path = folderModels.Last().Path,
            OwnerId = MockUtils.MOCK_USER_ID,
            Checksum = string.Empty,
            StorageKey = string.Empty,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
        _dbContext.Folders.AddRange(folderModels);
        _dbContext.Files.Add(fileModel);
        await _dbContext.SaveChangesAsync();
    }

    private async Task<IList<FolderModel>> GetFoldersToCreateAsync(string basePath, string filePath)
    {
        var folderModels = new List<FolderModel>();
        var fullPath = PathUtils.ConcatPath(basePath, filePath);
        var pathParts = PathUtils.SplitPath(fullPath);
        for (int i = pathParts.Length - 1; i > 0; i--)
        {
            var parentParts = pathParts.Take(i - 1).ToArray();
            var parentPath = PathUtils.CombinePath(parentParts);
            var parentPathName = pathParts.ElementAt(i);
            var parentFolder = await _dbContext.Folders
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Path == parentPath && f.Name == parentPathName);

            if (parentFolder != null)
            {
                break;
            }
            else
            {
                var newParentFolder = new FolderModel
                {
                    Id = Guid.NewGuid(),
                    Name = parentPathName,
                    Path = parentPath,
                    OwnerId = MockUtils.MOCK_USER_ID,
                    ParentFolderId = folderModels.Last().Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                };
                folderModels.Add(newParentFolder);
            }
        }
        return folderModels;
    }

    private async Task UploadFileAsync(UploadFilesCommand command)
    {
        var uploadDtos = command.FilesData.Select(data => new FileUploadDto(
                ContainerName: Constants.CONTAINER_NAME_FILES,
                BlobName: PathUtils.ConcatPath(command.BasePath, data.Path, data.FileName),
                Content: data.FileStream,
                ContentType: data.MimeType,
                Progress: (progress) =>
                {
                    Console.WriteLine($"Uploading {data.FileName}: {progress} bytes uploaded.");
                })
        );
        await _fileStorageService.UploadBulkAsync(uploadDtos, CancellationToken.None);
    }
}
