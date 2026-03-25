using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Files;

namespace TwoDrive.Api.Endpoints;

public sealed class UpdateFileRequest
{
    [Required]
    public Guid? FileId { get; init; }
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string? NewName { get; init; }
}


public sealed class UpdateFileEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("/files", HandleAsync)
            .WithName("UpdateFile")
            .WithTags("Files");
    }

    private static async Task<IResult> HandleAsync([FromBody] UpdateFileRequest request, [FromServices] ICommandHandler<UpdateFileCommand> handler)
    {
        try
        {
            await handler.Handle(new UpdateFileCommand
            {
                FileId = request.FileId!.Value,
                NewName = request.NewName
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
