using TwoDrive.Core.Models;

namespace TwoDrive.Services.__Persistence__;

public interface IFoldersRepository : IRepository
{
    /// <summary>
    /// Get folder by path. Ex: /a/b/c => get folder c in b in a. If the path is not found, return null.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    Task<FolderModel?> FindByPathAsync(string path);
    Task<FolderModel?> GetByIdAsync(Guid folderId);
    Task<Guid> CreateAsync(FolderModel folder);
    Task BulkCreateAsync(IEnumerable<FolderModel> folders);
    Task UpdateAsync(FolderModel folder);
    Task DeleteAsync(Guid id);
}
