using TwoDrive.Core;
using TwoDrive.Core.Helper;
using TwoDrive.Core.Models;
using TwoDrive.Services.__Persistence__;
using TwoDrive.Services.__Services__;
using TwoDrive.Services.Common;

namespace TwoDrive.Services.Files;

public class UploadFilesCommand : ICommand
{
    public class Item
    {
        public string Path { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string FileExtension { get; set; } = string.Empty;
        public string MimeType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public Stream FileStream { get; set; } = null!;
    }

    public string BasePath { get; set; } = string.Empty;
    public List<Item> FilesData { get; set; } = [];
}

internal class UploadFilesCommandHandler(
    IFilesRepository filesRepository,
    IFoldersRepository foldersRepository,
    IUnitOfWork unitOfWork,
    IFileStorageService fileStorageService
        ) : ICommandHandler<UploadFilesCommand>
{
    public async Task Handle(UploadFilesCommand command)
    {
        try
        {
            var storageKeysList = command.FilesData.Select(_ => Guid.NewGuid().ToString()).ToList();
            var filesDataWithStorageKeys = command.FilesData.Zip(storageKeysList, (fileData, storageKey) => (fileData, storageKey)).ToList();

            var uploadFileTask = UploadFileAsync(filesDataWithStorageKeys);
            var persistFileTask = Task.Run(async () =>
            {
                foreach (var (fileData, storageKey) in filesDataWithStorageKeys)
                {
                    var parentFolder = await CreateMissingFoldersAsync(command.BasePath, fileData.Path);
                    await CreateFileAsync(parentFolder, fileData, storageKey);
                }
            });
            
            await Task.WhenAll([persistFileTask, uploadFileTask]);
            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="parentFolder">If null, then create file at root</param>
    /// <param name="fileData"></param>
    /// <returns></returns>
    private async Task CreateFileAsync(FolderModel? parentFolder, UploadFilesCommand.Item fileData, string storageKey)
    {
        var fileModel = new FileModel
        {
            Id = Guid.NewGuid(),
            Name = fileData.FileName,
            Extension = fileData.FileExtension,
            MimeType = fileData.MimeType,
            SizeBytes = fileData.FileSizeBytes,
            FolderId = parentFolder?.Id,
            Path = parentFolder?.PathForChildren ?? CoreConstants.ROOT_PATH,
            OwnerId = MockUtils.MOCK_USER_ID,
            Checksum = string.Empty,
            StorageKey = storageKey,
        };
        await filesRepository.CreateAsync(fileModel);
    }

    /// <summary>
    /// Create missing folders in the given path if they don't exist, and return the parent folder of the file.
    /// </summary>
    /// <param name="basePath"></param>
    /// <param name="filePath"></param>
    /// <returns>Return the parent folder of the file, which can be null if the file is created at root.</returns>
    private async Task<FolderModel?> CreateMissingFoldersAsync(string basePath, string filePath)
    {
        FolderModel? fileParentFolder = null;
        var foldersToCreate = new List<FolderModel>();
        var fullPath = PathUtils.ConcatPath(basePath, filePath);
        var pathParts = PathUtils.SplitPath(fullPath);
        for (int i = pathParts.Length; i > 0; i--)
        {
            var currentPath = PathUtils.CombinePath(pathParts.Take(i).ToArray());
            var folderOfPath = await foldersRepository.GetByPathAsync(currentPath);
            if (folderOfPath != null)
            {
                // If we haven't found any missing folder so far, it means the file's parent folder already exists.
                if (foldersToCreate.Count == 0)
                {
                    fileParentFolder = folderOfPath;
                }

                // Found an existing folder in the path, so we can stop here. All parent folders above this already exist.
                break;
            }
            else
            {
                var parentParts = pathParts.Take(i - 1).ToArray();
                var parentPath = PathUtils.CombinePath(parentParts);
                var parentPathName = pathParts.ElementAt(i - 1);

                var newParentFolder = new FolderModel
                {
                    Id = Guid.NewGuid(),
                    Name = parentPathName,
                    Path = parentPath,
                    OwnerId = MockUtils.MOCK_USER_ID,
                    ParentFolderId = null,
                };
                foldersToCreate.Add(newParentFolder);
            }
        }

        // Set parent folder ID back
        // List order is: Child => ... => Parent
        for (int i = 0; i < foldersToCreate.Count - 1; i++)
        {
            foldersToCreate[i].ParentFolderId = foldersToCreate[i + 1].Id;
        }
        await foldersRepository.BulkCreateAsync(foldersToCreate);

        // Return the parent folder of the file, which can be null if the file is created at root.
        return fileParentFolder ?? foldersToCreate.FirstOrDefault() ?? null;
    }

    private async Task UploadFileAsync(List<(UploadFilesCommand.Item fileData, string storageKey)> filesDataWithStorageKeys)
    {
        var uploadDtos = filesDataWithStorageKeys.Select(data => new FileUploadDto(
                ContainerName: CoreConstants.CONTAINER_NAME_FILES,
                BlobName: data.storageKey,
                Content: data.fileData.FileStream,
                ContentType: data.fileData.MimeType,
                Progress: (progress) =>
                {
                    Console.WriteLine($"Uploading {data.fileData.FileName}: {progress} bytes uploaded.");
                })
        );
        await fileStorageService.UploadBulkAsync(uploadDtos, CancellationToken.None);
    }
}
