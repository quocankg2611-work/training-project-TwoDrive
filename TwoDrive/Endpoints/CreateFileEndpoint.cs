using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Files;

namespace TwoDrive.Endpoints;

public sealed class CreateFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/files", HandleAsync)
            .WithName("CreateFile")
            .WithTags("Files");
    }

    private static async Task<IResult> HandleAsync([FromBody] Request request, [FromServices] ICommandHandler<CreateFileCommand, CreateFileCommandResult> handler)
    {
        try
        {
            var result = await handler.Handle(new CreateFileCommand
            {
                FolderId = request.FolderId!.Value,
                Name = request.Name!,
                MimeType = request.MimeType!,
                SizeBytes = request.SizeBytes!.Value,
                StorageKey = request.StorageKey!,
                Checksum = request.Checksum!
            });

            var response = new Response(result.FileId);
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
        [StringLength(255, MinimumLength = 1)]
        public string? Name { get; init; }

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string? MimeType { get; init; }

        [Required]
        [Range(0, long.MaxValue)]
        public long? SizeBytes { get; init; }

        [Required]
        [StringLength(1024, MinimumLength = 1)]
        public string? StorageKey { get; init; }

        [Required]
        [StringLength(1024, MinimumLength = 1)]
        public string? Checksum { get; init; }
    }

    public sealed record Response(Guid FileId);
}
