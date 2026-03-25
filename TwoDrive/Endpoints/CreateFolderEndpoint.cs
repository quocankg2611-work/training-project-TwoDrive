using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Services.Common;
using TwoDrive.Services.Folders;

namespace TwoDrive.Api.Endpoints;

public sealed class CreateFolderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/folders", HandleAsync)
            .WithName("CreateFolder")
            .WithTags("Folders")
            .Produces<CreateFolderResponse>(StatusCodes.Status200OK)
            ;
    }

    private static async Task<IResult> HandleAsync([FromBody] CreateFolderRequest request, [FromServices] ICommandHandler<CreateFolderCommand, CreateFolderCommandResult> handler)
    {
        var result = await handler.Handle(new CreateFolderCommand
        {
            ParentFolderId = request.ParentFolderId,
            Name = request.Name!
        });

        var response = new CreateFolderResponse(result.FolderId);
        return Results.Created($"/folders/{result.FolderId}", response);
    }
}

public sealed class CreateFolderRequest
{
    public Guid? ParentFolderId { get; init; }

    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string? Name { get; init; }
}

public sealed record CreateFolderResponse(Guid FolderId);