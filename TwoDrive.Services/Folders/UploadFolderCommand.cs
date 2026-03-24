using TwoDrive.Services.Common;

namespace TwoDrive.Services.Folders;

public class UploadFolderCommand : ICommand
{
    public Guid FolderId { get; set; }
}
