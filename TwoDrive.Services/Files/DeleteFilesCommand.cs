using TwoDrive.Services.Common;
using TwoDrive.Services.__Persistence__;
using TwoDrive.Services.__Services__;

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
    public Task Handle(DeleteFilesCommand command)
    {
        throw new NotImplementedException();
    }
}
