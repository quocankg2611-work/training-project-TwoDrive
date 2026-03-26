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
    public async Task Handle(UpdateFolderCommand command)
    {
        var folder = await foldersRepository.GetByIdAsync(command.FolderId) ?? throw new KeyNotFoundException($"Folder with key {command.FolderId} not found");
        if (string.IsNullOrWhiteSpace(command.NewName) == false)
        {
            folder.Name = command.NewName;
        }
        await foldersRepository.UpdateAsync(folder);
    }
}
