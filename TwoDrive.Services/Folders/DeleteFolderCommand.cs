using TwoDrive.Services.Common;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Services.Folders;

public class DeleteFolderCommand : ICommand
{
    public Guid FolderId { get; set; }
    public bool Recursive { get; set; }
}

internal class DeleteFolderCommandHandler : ICommandHandler<DeleteFolderCommand>
{
    private readonly IFoldersRepository _foldersRepository;

    public DeleteFolderCommandHandler(IFoldersRepository foldersRepository)
    {
        _foldersRepository = foldersRepository;
    }

    public Task Handle(DeleteFolderCommand command)
    {
        return _foldersRepository.DeleteAsync(command);
    }
}
