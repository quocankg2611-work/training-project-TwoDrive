using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Files;

namespace TwoDrive.Api.Endpoints;

public sealed class UploadFilesRequest
{
    [Required]
    [MinLength(1)]
    public string? BasePath { get; init; }

    [Required]
    [MinLength(1)]
    public IFormFileCollection Files { get; init; } = default!;

    /// <summary>
    /// Calculated file path from the client side relative to the base path.
    /// For multiple files upload to different folder. (Folder upload)
    /// Must be in the same order as the files in the Files property.
    /// </summary>
    [Required]
    [MinLength(1)]
    public string[] FilePaths { get; init; } = [];
}

public sealed class UploadFilesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/files/upload", HandleAsync)
            .WithName("UploadFiles")
            .Accepts<UploadFilesRequest>("multipart/form-data")
            .WithTags("Files")
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK)
            .DisableAntiforgery() // CSRF disabled
            ;
    }

    private static async Task<IResult> HandleAsync([FromForm] UploadFilesRequest request, [FromServices] ICommandHandler<UploadFilesCommand> handler)
    {
        var streams = new List<MemoryStream>();

        try
        {
            var filesData = new List<UploadFilesCommand.Item>();
            for (int i = 0; i < request.Files.Count; i++)
            {
                var item = request.Files[i];
                var path = request.FilePaths[i];

                await using var sourceStream = item.OpenReadStream();
                var copiedStream = new MemoryStream();
                await sourceStream.CopyToAsync(copiedStream);
                copiedStream.Position = 0;

                streams.Add(copiedStream);

                var originalFileName = Path.GetFileName(item.FileName);
                filesData.Add(new UploadFilesCommand.Item
                {
                    Path = path,
                    FileName = Path.GetFileNameWithoutExtension(originalFileName),
                    FileExtension = Path.GetExtension(originalFileName),
                    MimeType = item.ContentType,
                    FileSizeBytes = item.Length,
                    FileStream = copiedStream
                });
            }

            await handler.Handle(new UploadFilesCommand
            {
                BasePath = request.BasePath!,
                FilesData = filesData
            });

            return Results.Ok();
        }
        finally
        {
            foreach (var stream in streams)
            {
                await stream.DisposeAsync();
            }
        }
    }
}

