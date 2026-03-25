using TwoDrive.Services.Common;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Services.Folders;

public class DeleteFoldersCommand : ICommand
{
    public IEnumerable<Guid> FolderIds { get; set; } = [];

}

internal class DeleteFolderCommandHandler(IFoldersRepository foldersRepository) : ICommandHandler<DeleteFoldersCommand>
{
    public Task Handle(DeleteFoldersCommand command)
    {
        throw new NotImplementedException();
    }
}
