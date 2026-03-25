using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Persistence.Queries;
using TwoDrive.Services.Common;

namespace TwoDrive.Api.Endpoints;

public sealed class GetDocumentsByPathEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/documents", HandleAsync)
            .WithName("GetDocumentsByPath")
            .WithTags("Documents")
            .Produces<GetDocumentsByPathResponse>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> HandleAsync([AsParameters] Request request, [FromServices] IQueryHandler<GetDocumentsByPathQuery, IEnumerable<GetDocumentsQueryResultItem>> handler)
    {
        var results = await handler.Handle(new GetDocumentsByPathQuery
        {
            Path = request.Path!
        });

        var response = new GetDocumentsByPathResponse(
            results.Select(item => new GetDocumentsByPathResponseItem(
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
        [FromQuery]
        public string? Path { get; init; }
    }
}

public sealed record GetDocumentsByPathResponseItem(
    Guid Id,
    string Name,
    string Path,
    string DocumentType,
    string? FileType,
    string ModifiedBy,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public sealed record GetDocumentsByPathResponse(IReadOnlyCollection<GetDocumentsByPathResponseItem> Items);

