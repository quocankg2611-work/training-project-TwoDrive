using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Folders;

namespace TwoDrive.Endpoints;

public sealed class DeleteFolderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/folders/{folderId:guid}", HandleAsync)
            .WithName("DeleteFolder")
            .WithTags("Folders");
    }

    private static async Task<IResult> HandleAsync([AsParameters] Request request, [FromServices] ICommandHandler<DeleteFolderCommand> handler)
    {
        try
        {
            await handler.Handle(new DeleteFolderCommand
            {
                FolderId = request.FolderId!.Value,
                Recursive = request.Recursive
            });

            return Results.Ok(new Response("Folder deleted."));
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

        public bool Recursive { get; init; }
    }

    private sealed record Response(string Message);
}
