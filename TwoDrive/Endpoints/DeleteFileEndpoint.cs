using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Files;

namespace TwoDrive.Endpoints;

public sealed class DeleteFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/files/{fileId:guid}", HandleAsync)
            .WithName("DeleteFile")
            .WithTags("Files");
    }

    private static async Task<IResult> HandleAsync([AsParameters] Request request, [FromServices] ICommandHandler<DeleteFileCommand> handler)
    {
        try
        {
            await handler.Handle(new DeleteFileCommand { FileId = request.FileId!.Value });
            return Results.Ok(new Response("File deleted."));
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }

    public sealed class Request
    {
        [Required]
        public Guid? FileId { get; init; }
    }

    private sealed record Response(string Message);
}
