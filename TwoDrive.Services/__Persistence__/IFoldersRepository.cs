using TwoDrive.Services.Folders;

namespace TwoDrive.Services.__Persistence__;

public interface IFoldersRepository
{
    Task<CreateFolderCommandResult> CreateAsync(CreateFolderCommand command);
    Task<FolderDetailsDto?> GetByIdAsync(Guid folderId);
    Task UpdateAsync(UpdateFolderCommand command);
    Task DeleteAsync(DeleteFolderCommand command);
    Task EnsureExistsAsync(Guid folderId);
}
