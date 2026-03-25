using TwoDrive.Services.Common;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Services.Folders;

public class GetFolderByIdQuery : IQuery<FolderDetailsDto>
{
    public Guid FolderId { get; set; }
}

public record FolderDetailsDto(
    Guid Id,
    Guid? ParentFolderId,
    string Name,
    string Path,
    DateTime CreatedAt,
    DateTime UpdatedAt);

internal class GetFolderByIdQueryHandler : IQueryHandler<GetFolderByIdQuery, FolderDetailsDto>
{
    private readonly IFoldersRepository _foldersRepository;

    public GetFolderByIdQueryHandler(IFoldersRepository foldersRepository)
    {
        _foldersRepository = foldersRepository;
    }

    public async Task<FolderDetailsDto> Handle(GetFolderByIdQuery query)
    {
        return await _foldersRepository.GetByIdAsync(query.FolderId)
            ?? throw new KeyNotFoundException($"Folder '{query.FolderId}' was not found.");
    }
}
