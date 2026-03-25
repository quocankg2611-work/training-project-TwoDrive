using TwoDrive.Services.Common;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Services.Folders;

public class UploadFolderCommand : ICommand
{
    public Guid FolderId { get; set; }
}

internal class UploadFolderCommandHandler : ICommandHandler<UploadFolderCommand>
{
    private readonly IFoldersRepository _foldersRepository;

    public UploadFolderCommandHandler(IFoldersRepository foldersRepository)
    {
        _foldersRepository = foldersRepository;
    }

    public async Task Handle(UploadFolderCommand command)
    {
        await _foldersRepository.EnsureExistsAsync(command.FolderId);
    }
}
