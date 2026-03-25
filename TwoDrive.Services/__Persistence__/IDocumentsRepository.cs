using TwoDrive.Services.Documents;

namespace TwoDrive.Services.__Persistence__;

public interface IDocumentsRepository
{
    Task<IEnumerable<GetDocumentsQueryResultItem>> GetByPathAsync(string path);
}
