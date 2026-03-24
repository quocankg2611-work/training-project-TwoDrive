using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Files;

namespace TwoDrive.Endpoints;

public sealed class UploadFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/upload", HandleAsync)
            .WithName("UploadFile")
            .WithTags("Files");
    }

    private static async Task<IResult> HandleAsync([FromForm] Request request, [FromServices] ICommandHandler<UploadFileCommand, UploadFileCommandResult> handler)
    {
        try
        {
            await using var fileStream = request.File!.OpenReadStream();

            var result = await handler.Handle(new UploadFileCommand
            {
                FolderId = request.FolderId!.Value,
                FileName = request.File.FileName,
                MimeType = string.IsNullOrWhiteSpace(request.File.ContentType)
                    ? "application/octet-stream"
                    : request.File.ContentType,
                FileSizeBytes = request.File.Length,
                FileStream = fileStream
            });

            var response = new Response(result.FileId, result.StorageKey);
            return Results.Created($"/files/{result.FileId}", response);
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

        [Required]
        public IFormFile? File { get; init; }
    }

    private sealed record Response(Guid FileId, string StorageKey);
}
