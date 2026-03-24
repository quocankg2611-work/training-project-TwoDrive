using TwoDrive.Services.Common;

namespace TwoDrive.Services.Files;

public class DeleteFileCommand : ICommand
{
    public Guid FileId { get; set; }
}
