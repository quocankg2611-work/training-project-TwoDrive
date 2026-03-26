using TwoDrive.Core.Helper;
using TwoDrive.Core.Models;
using TwoDrive.Services.__Persistence__;
using TwoDrive.Services.Common;

namespace TwoDrive.Services.Folders;

public class CreateFolderCommand : ICommand
{
    public Guid? ParentFolderId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public record CreateFolderCommandResult(Guid FolderId);

internal class CreateFolderCommandHandler(
    IFoldersRepository foldersRepository,
    IUnitOfWork unitOfWork
    ) : ICommandHandler<CreateFolderCommand, CreateFolderCommandResult>
{
    public async Task<CreateFolderCommandResult> Handle(CreateFolderCommand command)
    {
        var parentFolder = command.ParentFolderId.HasValue
            ? await foldersRepository.GetByIdAsync(command.ParentFolderId.Value)
            : null;

        var folderModel = new FolderModel
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Path = parentFolder?.PathForChildren ?? "/",
            ParentFolderId = parentFolder?.Id,
        };

        var folderId = await foldersRepository.CreateAsync(folderModel);
        await unitOfWork.SaveChangesAsync();
        return new CreateFolderCommandResult(folderId);
    }
}
