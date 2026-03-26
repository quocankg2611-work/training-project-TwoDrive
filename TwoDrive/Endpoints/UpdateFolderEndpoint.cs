using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Folders;

namespace TwoDrive.Api.Endpoints;

public sealed class UpdateFolderRequest
{
    [Required]
    public Guid? FolderId { get; init; }
    [StringLength(255, MinimumLength = 1)]
    public string? NewName { get; init; }
}

public sealed class UpdateFolderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/folders", HandleAsync)
            .WithName("UpdateFolder")
            .WithTags("Folders")
            .RequireAuthorization()
            .Produces(StatusCodes.Status200OK);
    }

    private static async Task<IResult> HandleAsync([FromBody] UpdateFolderRequest request, [FromServices] ICommandHandler<UpdateFolderCommand> handler)
    {
        await handler.Handle(new UpdateFolderCommand
        {
            FolderId = request.FolderId!.Value,
            NewName = request.NewName,
        });

        return Results.Ok();
    }
}
