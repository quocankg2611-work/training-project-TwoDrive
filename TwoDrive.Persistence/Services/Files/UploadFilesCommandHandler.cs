using Microsoft.EntityFrameworkCore;
using TwoDrive.Core;
using TwoDrive.Core.Helper;
using TwoDrive.Persistence.Models;
using TwoDrive.Services.__Services__;
using TwoDrive.Services.Common;
using TwoDrive.Services.Files;

namespace TwoDrive.Persistence.Services.Files;

internal class UploadFilesCommandHandler(
    AppDbContext dbContext,
    IFileStorageService fileStorageService
    ) : ICommandHandler<UploadFilesCommand>
{
    public async Task Handle(UploadFilesCommand command)
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
        dbContext.Folders.AddRange(folderModels);
        dbContext.Files.Add(fileModel);
        await dbContext.SaveChangesAsync();
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
            var parentFolder = await dbContext.Folders
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Path == parentPath && f.Name == parentPathName);

            // Found parent folder, stop creating
            if (parentFolder != null)
            {
                break;
            }
            // Not found, create new parent folder and continue to find next parent
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
                    // You can implement progress tracking logic here, e.g., update a database record or send progress updates to clients
                    Console.WriteLine($"Uploading {data.FileName}: {progress} bytes uploaded.");
                })
        );
        await fileStorageService.UploadBulkAsync(uploadDtos, CancellationToken.None);
    }
}
