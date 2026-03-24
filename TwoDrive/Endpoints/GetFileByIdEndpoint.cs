using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Files;

namespace TwoDrive.Endpoints;

public sealed class GetFileByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/{fileId:guid}", HandleAsync)
            .WithName("GetFileById")
            .WithTags("Files");
    }

    private static async Task<IResult> HandleAsync([AsParameters] Request request, [FromServices] IQueryHandler<GetFileByIdQuery, FileDetailsDto> handler)
    {
        try
        {
            var file = await handler.Handle(new GetFileByIdQuery { FileId = request.FileId!.Value });
            var response = new Response(
                file.Id,
                file.FolderId,
                file.Name,
                file.MimeType,
                file.SizeBytes,
                file.StorageKey,
                file.Checksum,
                file.CreatedAt,
                file.UpdatedAt);

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
        public Guid? FileId { get; init; }
    }

    private sealed record Response(
        Guid Id,
        Guid FolderId,
        string Name,
        string MimeType,
        long SizeBytes,
        string StorageKey,
        string Checksum,
        DateTime CreatedAt,
        DateTime UpdatedAt);
}
