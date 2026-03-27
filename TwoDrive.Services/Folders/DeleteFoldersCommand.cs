using TwoDrive.Core;
using TwoDrive.Services.__Persistence__;
using TwoDrive.Services.__Services__;
using TwoDrive.Services.Common;

namespace TwoDrive.Services.Folders;

public class DeleteFoldersCommand : ICommand
{
    public IEnumerable<Guid> FolderIds { get; set; } = [];

}

internal class DeleteFolderCommandHandler(
    IFoldersRepository foldersRepository,
    IFilesRepository filesRepository,
    IFileStorageService fileStorageService,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteFoldersCommand>
{
    public async Task Handle(DeleteFoldersCommand command)
    {
        var folders = await foldersRepository.GetByIdsAsync(command.FolderIds);

        // To remove the files from storage
        var filesToDelete = await filesRepository.QueryDeepChildrenOfFolders(folders);

        var storageTask = fileStorageService.DeleteBulkAsync(CoreConstants.CONTAINER_NAME_FILES, filesToDelete.Select(f => f.StorageKey));
        var persistenceTask = foldersRepository.BulkDeleteAsync(command.FolderIds);

        await Task.WhenAll(storageTask, persistenceTask);
        await unitOfWork.SaveChangesAsync();
    }
}
