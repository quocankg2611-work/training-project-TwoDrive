using TwoDrive.Services.Common;

namespace TwoDrive.Services.Folders;

public class DeleteFolderCommand : ICommand
{
    public Guid FolderId { get; set; }
    public bool Recursive { get; set; }
}
