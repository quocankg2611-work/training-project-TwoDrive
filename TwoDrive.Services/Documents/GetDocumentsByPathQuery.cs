using TwoDrive.Services.Common;
using TwoDrive.Services.__Persistence__;

namespace TwoDrive.Services.Documents;

public class GetDocumentsByPathQuery : IQuery<IEnumerable<GetDocumentsQueryResultItem>>
{
    public string Path { get; set; } = null!;
}

public record GetDocumentsQueryResultItem(
    Guid Id,
    string Name,
    string Path,
    string DocumentType,
    string? FileType,
    string ModifiedBy,
    DateTime CreatedAt,
    DateTime UpdatedAt
    );

internal class GetDocumentsByPathQueryHandler : IQueryHandler<GetDocumentsByPathQuery, IEnumerable<GetDocumentsQueryResultItem>>
{
    private readonly IDocumentsRepository _documentsRepository;

    public GetDocumentsByPathQueryHandler(IDocumentsRepository documentsRepository)
    {
        _documentsRepository = documentsRepository;
    }

    public Task<IEnumerable<GetDocumentsQueryResultItem>> Handle(GetDocumentsByPathQuery query)
    {
        return _documentsRepository.GetByPathAsync(query.Path);
    }
}
