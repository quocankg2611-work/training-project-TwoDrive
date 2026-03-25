using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Persistence.Queries;
using TwoDrive.Services.Common;

namespace TwoDrive.Api.Endpoints;

public sealed record GetFolderByIdResponse(
    Guid Id,
    Guid? ParentFolderId,
    string Name,
    string Path,
    DateTime CreatedAt,
    DateTime UpdatedAt
    );

public sealed class GetFolderByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/folders/{folderId:guid}", HandleAsync)
            .WithName("GetFolderById")
            .WithTags("Folders");
    }

    private static async Task<IResult> HandleAsync([FromRoute] Guid FolderId, [FromServices] IQueryHandler<GetFolderByIdQuery, FolderDetailsDto> handler)
    {
        try
        {
            var folder = await handler.Handle(new GetFolderByIdQuery { FolderId = FolderId });
            var response = new GetFolderByIdResponse(
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
}

