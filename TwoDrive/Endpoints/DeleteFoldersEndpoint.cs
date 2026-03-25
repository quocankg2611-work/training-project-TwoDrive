using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Folders;

namespace TwoDrive.Api.Endpoints;

public sealed class DeleteFoldersRequest
{
    [Required]
    [MinLength(1)]
    public Guid[]? FolderIds { get; init; }
}

public sealed class DeleteFoldersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("/folders", HandleAsync)
            .WithName("DeleteFolders")
            .WithTags("Folders");
    }

    private static async Task<IResult> HandleAsync([FromBody] DeleteFoldersRequest request, [FromServices] ICommandHandler<DeleteFoldersCommand> handler)
    {
        try
        {
            await handler.Handle(new DeleteFoldersCommand
            {
                FolderIds = request.FolderIds!
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

