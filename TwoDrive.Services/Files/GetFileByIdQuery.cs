using TwoDrive.Services.Common;
using TwoDrive.Services.__Persistence__;

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

internal class GetFileByIdQueryHandler : IQueryHandler<GetFileByIdQuery, FileDetailsDto>
{
    private readonly IFilesRepository _filesRepository;

    public GetFileByIdQueryHandler(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }

    public async Task<FileDetailsDto> Handle(GetFileByIdQuery query)
    {
        return await _filesRepository.GetByIdAsync(query.FileId)
            ?? throw new KeyNotFoundException($"File '{query.FileId}' was not found.");
    }
}
