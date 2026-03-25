using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Files;

namespace TwoDrive.Api.Endpoints;

public sealed class DeleteFilesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/files", HandleAsync)
            .WithName("DeleteFiles")
            .WithTags("Files");
    }

    private static async Task<IResult> HandleAsync([FromBody] DeleteFilesRequest request, [FromServices] ICommandHandler<DeleteFilesCommand> handler)
    {
        try
        {
            await handler.Handle(new DeleteFilesCommand
            {
                FileIds = request.FileIds!
            });

            return Results.Ok();
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
}

public sealed class DeleteFilesRequest
{
    [Required]
    [MinLength(1)]
    public Guid[]? FileIds { get; init; }
}
