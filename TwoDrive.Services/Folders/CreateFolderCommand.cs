using TwoDrive.Services.Common;

namespace TwoDrive.Services.Folders;

public class CreateFolderCommand : ICommand
{
    public Guid? ParentFolderId { get; set; }
    public string Name { get; set; } = string.Empty;
}

public record CreateFolderCommandResult(Guid FolderId);
