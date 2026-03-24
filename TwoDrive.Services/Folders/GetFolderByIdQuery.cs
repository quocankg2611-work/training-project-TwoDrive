using TwoDrive.Services.Common;

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
