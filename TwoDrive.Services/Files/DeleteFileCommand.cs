using TwoDrive.Services.Common;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Services.Files;

public class DeleteFileCommand : ICommand
{
    public Guid FileId { get; set; }
}

internal class DeleteFileCommandHandler : ICommandHandler<DeleteFileCommand>
{
    private readonly IFilesRepository _filesRepository;

    public DeleteFileCommandHandler(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public Task Handle(DeleteFileCommand command)
    {
        return _filesRepository.DeleteAsync(command.FileId);
    }
}
