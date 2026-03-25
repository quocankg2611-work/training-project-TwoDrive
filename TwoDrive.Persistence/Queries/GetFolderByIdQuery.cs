using TwoDrive.Services.Common;

namespace TwoDrive.Persistence.Queries;

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
    public Task<FolderDetailsDto> Handle(GetFolderByIdQuery query)
    {
        throw new NotImplementedException();
    }
}