using TwoDrive.Services.Common;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Services.Folders;

public class UpdateFolderCommand : ICommand
{
    public Guid FolderId { get; set; }
    public string? NewName { get; set; }
}

internal class UpdateFolderCommandHandler(IFoldersRepository foldersRepository) : ICommandHandler<UpdateFolderCommand>
{
    public Task Handle(UpdateFolderCommand command)
    {
        throw new NotImplementedException();
    }
}
