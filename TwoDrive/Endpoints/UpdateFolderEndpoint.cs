using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Folders;

namespace TwoDrive.Endpoints;

public sealed class UpdateFolderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/folders", HandleAsync)
            .WithName("UpdateFolder")
            .WithTags("Folders");
    }

    private static async Task<IResult> HandleAsync([FromBody] Request request, [FromServices] ICommandHandler<UpdateFolderCommand> handler)
    {
        try
        {
            await handler.Handle(new UpdateFolderCommand
            {
                FolderId = request.FolderId!.Value,
                NewName = request.NewName,
                NewParentFolderId = request.NewParentFolderId
            });

            return Results.Ok(new Response("Folder updated."));
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(new { message = ex.Message });
        }
    }

    public sealed class Request
    {
        [Required]
        public Guid? FolderId { get; init; }

        [StringLength(255, MinimumLength = 1)]
        public string? NewName { get; init; }

        public Guid? NewParentFolderId { get; init; }
    }

    private sealed record Response(string Message);
}
