using TwoDrive.Services.Common;

namespace TwoDrive.Services.Files;

public class GetFileByIdQuery : IQuery<FileDetailsDto>
{
    public Guid FileId { get; set; }
}

public record FileDetailsDto(
    Guid Id,
    Guid FolderId,
    string Name,
    string MimeType,
    long SizeBytes,
    string StorageKey,
    string Checksum,
    DateTime CreatedAt,
    DateTime UpdatedAt);
