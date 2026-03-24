using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Documents;

namespace TwoDrive.Endpoints;

public sealed class GetDocumentsByPathEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/documents", HandleAsync)
            .WithName("GetDocumentsByPath")
            .WithTags("Documents");
    }

    private static async Task<IResult> HandleAsync([AsParameters] Request request, [FromServices] IQueryHandler<GetDocumentsByPathQuery, IEnumerable<GetDocumentsQueryResultItem>> handler)
    {
        var results = await handler.Handle(new GetDocumentsByPathQuery
        {
            Path = request.Path!
        });

        var response = new Response(
            results.Select(item => new ResponseItem(
                item.Id,
                item.Name,
                item.Path,
                item.DocumentType,
                item.FileType,
                item.ModifiedBy,
                item.CreatedAt,
                item.UpdatedAt)).ToArray());

        return Results.Ok(response);
    }

    public sealed class Request
    {
        [Required]
        [MinLength(1)]
        public string? Path { get; init; }
    }

    private sealed record Response(IReadOnlyCollection<ResponseItem> Items);

    private sealed record ResponseItem(
        Guid Id,
        string Name,
        string Path,
        string DocumentType,
        string? FileType,
        string ModifiedBy,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
