using TwoDrive.Services.Common;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Services.Folders;

public class CreateFolderCommand : ICommand
{
    public Guid? ParentFolderId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public record CreateFolderCommandResult(Guid FolderId);

internal class CreateFolderCommandHandler : ICommandHandler<CreateFolderCommand, CreateFolderCommandResult>
{
    private readonly IFoldersRepository _foldersRepository;

    public CreateFolderCommandHandler(IFoldersRepository foldersRepository)
    {
        _foldersRepository = foldersRepository;
    }

    public Task<CreateFolderCommandResult> Handle(CreateFolderCommand command)
    {
        return _foldersRepository.CreateAsync(command);
    }
}
