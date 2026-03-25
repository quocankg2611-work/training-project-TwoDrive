using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Persistence.Queries;
using TwoDrive.Services.Common;

namespace TwoDrive.Api.Endpoints;

public sealed record GetFileByIdResponse(
        Guid Id,
        Guid FolderId,
        string Name,
        string MimeType,
        long SizeBytes,
        string StorageKey,
        string Checksum,
        DateTime CreatedAt,
        DateTime UpdatedAt);

public sealed class GetFileByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/{fileId:guid}", HandleAsync)
            .WithName("GetFileById")
            .WithTags("Files")
            .Produces<GetFileByIdResponse>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> HandleAsync([FromRoute] Guid fileId, [FromServices] IQueryHandler<GetFileByIdQuery, FileDetailsDto> handler)
    {
        var file = await handler.Handle(new GetFileByIdQuery { FileId = fileId });
        var response = new GetFileByIdResponse(
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
}
