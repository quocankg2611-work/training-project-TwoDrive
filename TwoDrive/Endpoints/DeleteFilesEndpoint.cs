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
            .WithTags("Files")
            .Produces(StatusCodes.Status200OK);
    }

    private static async Task<IResult> HandleAsync([FromBody] DeleteFilesRequest request, [FromServices] ICommandHandler<DeleteFilesCommand> handler)
    {
        await handler.Handle(new DeleteFilesCommand
        {
            FileIds = request.FileIds!
        });

        return Results.Ok();
    }
}

public sealed class DeleteFilesRequest
{
    [Required]
    [MinLength(1)]
    public Guid[]? FileIds { get; init; }
}
