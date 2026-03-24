using TwoDrive.Services.Common;

namespace TwoDrive.Services.Files;

public class CreateFileCommand : ICommand
{
    public Guid FolderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public string Checksum { get; set; } = string.Empty;
}

public record CreateFileCommandResult(Guid FileId);
