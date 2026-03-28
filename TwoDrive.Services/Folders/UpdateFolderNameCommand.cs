using TwoDrive.Services.Common;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Services.Folders;

public class UpdateFolderNameCommand : ICommand
{
    public Guid FolderId { get; set; }
    public string NewName { get; set; } = string.Empty;
}

internal class UpdateFolderNameCommandHandler(
    IFoldersRepository foldersRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<UpdateFolderNameCommand>
{
    public async Task Handle(UpdateFolderNameCommand command)
    {
        await foldersRepository.UpdateFolderNameAsync(command.FolderId, command.NewName);
        await unitOfWork.SaveChangesAsync();
    }
}
