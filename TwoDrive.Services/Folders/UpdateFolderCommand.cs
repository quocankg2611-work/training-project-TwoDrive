using TwoDrive.Services.Common;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Services.Folders;

public class UpdateFolderCommand : ICommand
{
    public Guid FolderId { get; set; }
    public string? NewName { get; set; }
    public Guid? NewParentFolderId { get; set; }
}

internal class UpdateFolderCommandHandler : ICommandHandler<UpdateFolderCommand>
{
    private readonly IFoldersRepository _foldersRepository;

    public UpdateFolderCommandHandler(IFoldersRepository foldersRepository)
    {
        _foldersRepository = foldersRepository;
    }

    public Task Handle(UpdateFolderCommand command)
    {
        return _foldersRepository.UpdateAsync(command);
    }
}
