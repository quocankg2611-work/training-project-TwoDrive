using TwoDrive.Core.Models;

namespace TwoDrive.Services.__Persistence__;

public interface IFilesRepository : IRepository
{
    Task<IEnumerable<FileModel>> GetByIdsAsync(IEnumerable<Guid> ids);
    Task<FileModel?> GetByIdAsync(Guid id);
    Task CreateAsync(FileModel file);
    Task UpdateAsync(FileModel file);
    Task BulkDeleteAsync(IEnumerable<Guid> ids);
    Task<IEnumerable<FileModel>> QueryDeepChildrenOfFolders(IEnumerable<FolderModel> folderModels);
}
