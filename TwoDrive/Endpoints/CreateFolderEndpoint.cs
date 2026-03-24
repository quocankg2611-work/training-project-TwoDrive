using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Folders;

namespace TwoDrive.Endpoints;

public sealed class CreateFolderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/folders", HandleAsync)
            .WithName("CreateFolder")
            .WithTags("Folders");
    }

    private static async Task<IResult> HandleAsync([FromBody] Request request, [FromServices] ICommandHandler<CreateFolderCommand, CreateFolderCommandResult> handler)
    {
        try
        {
            var result = await handler.Handle(new CreateFolderCommand
            {
                ParentFolderId = request.ParentFolderId,
                Name = request.Name!
            });

            var response = new Response(result.FolderId);
            return Results.Created($"/folders/{result.FolderId}", response);
        }
        catch (KeyNotFoundException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }

    public sealed class Request
    {
        public Guid? ParentFolderId { get; init; }

        [Required]
        [StringLength(255, MinimumLength = 1)]
        public string? Name { get; init; }
    }

    public sealed record Response(Guid FolderId);
}
