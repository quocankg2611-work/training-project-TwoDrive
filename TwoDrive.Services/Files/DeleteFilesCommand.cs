using TwoDrive.Core;
using TwoDrive.Services.__Persistence__;
using TwoDrive.Services.__Services__;
using TwoDrive.Services.Common;

namespace TwoDrive.Services.Files;

public class DeleteFilesCommand : ICommand
{
    public IEnumerable<Guid> FileIds { get; set; } = [];
}

internal class DeleteFilesCommandHandler(
    IFilesRepository filesRepository,
    IFileStorageService fileStorageService
    ) : ICommandHandler<DeleteFilesCommand>
{
    public async Task Handle(DeleteFilesCommand command)
    {
        var files = await filesRepository.GetByIdsAsync(command.FileIds);
        var fileKeys = files.Select(f => f.StorageKey);

        var persistenceTask = filesRepository.BulkDeleteAsync(command.FileIds);
        var storageTask  = fileStorageService.DeleteBulkAsync(CoreConstants.CONTAINER_NAME_FILES, fileKeys);
        await Task.WhenAll(persistenceTask, storageTask);
    }
}
