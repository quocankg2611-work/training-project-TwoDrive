using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Folders;

namespace TwoDrive.Endpoints;

public sealed class GetFolderByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/folders/{folderId:guid}", HandleAsync)
            .WithName("GetFolderById")
            .WithTags("Folders");
    }

    private static async Task<IResult> HandleAsync([AsParameters] Request request, [FromServices] IQueryHandler<GetFolderByIdQuery, FolderDetailsDto> handler)
    {
        try
        {
            var folder = await handler.Handle(new GetFolderByIdQuery { FolderId = request.FolderId!.Value });
            var response = new Response(
                folder.Id,
                folder.ParentFolderId,
                folder.Name,
                folder.Path,
                folder.CreatedAt,
                folder.UpdatedAt);

            return Results.Ok(response);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }

    public sealed class Request
    {
        [Required]
        public Guid? FolderId { get; init; }
    }

    private sealed record Response(
        Guid Id,
        Guid? ParentFolderId,
        string Name,
        string Path,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
