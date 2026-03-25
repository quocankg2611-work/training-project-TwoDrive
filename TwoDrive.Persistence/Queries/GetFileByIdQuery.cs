using TwoDrive.Services.Common;
using TwoDrive.Persistence;

namespace TwoDrive.Persistence.Queries;

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

internal class GetFileByIdQueryHandler(AppDbContext dbContext) : IQueryHandler<GetFileByIdQuery, FileDetailsDto>
{
    public async Task<FileDetailsDto> Handle(GetFileByIdQuery query)
    {
        throw new NotImplementedException();
    }
}
