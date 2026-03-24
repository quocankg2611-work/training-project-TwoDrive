using TwoDrive.Services.Common;

namespace TwoDrive.Services.Folders;

public class UpdateFolderCommand : ICommand
{
    public Guid FolderId { get; set; }
    public string? NewName { get; set; }
    public Guid? NewParentFolderId { get; set; }
}
