using TwoDrive.Core.Models;

namespace TwoDrive.Services.__Persistence__;

public interface IFilesRepository : IRepository
{
    Task CreateAsync(FileModel file);
    Task UpdateAsync(FileModel file);
    Task DeleteAsync(Guid id);
}
