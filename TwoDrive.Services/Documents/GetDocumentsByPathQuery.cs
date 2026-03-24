using TwoDrive.Services.Common;

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
