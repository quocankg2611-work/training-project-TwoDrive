using TwoDrive.Services.Common;

namespace TwoDrive.Services.Files;

public class UploadFileCommand : ICommand
{
    public Guid FolderId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public Stream FileStream { get; set; } = null!;
}

public record UploadFileCommandResult(Guid FileId, string StorageKey);
