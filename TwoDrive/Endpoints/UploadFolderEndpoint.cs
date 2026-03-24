using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Folders;

namespace TwoDrive.Endpoints;

public sealed class UploadFolderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/folders/upload", HandleAsync)
            .WithName("UploadFolder")
            .WithTags("Folders");
    }

    private static async Task<IResult> HandleAsync([FromBody] Request request, [FromServices] ICommandHandler<UploadFolderCommand> handler)
    {
        try
        {
            await handler.Handle(new UploadFolderCommand
            {
                FolderId = request.FolderId!.Value
            });

            return Results.Ok(new Response("Folder upload accepted."));
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

    private sealed record Response(string Message);
}
