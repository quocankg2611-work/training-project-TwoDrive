using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;
using TwoDrive.Persistence.Queries;
using TwoDrive.Services.Common;

namespace TwoDrive.Api.Endpoints;

public sealed record GetFileByIdResponse : GetFileByIdQueryResult;

public sealed class GetFileByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/files/{fileId:guid}", HandleAsync)
            .WithName("GetFileById")
            .WithTags("Files")
            .RequireAuthorization()
            .Produces<GetFileByIdResponse>(StatusCodes.Status200OK);
    }

    private static async Task<IResult> HandleAsync([FromRoute] Guid fileId, [FromServices] IQueryHandler<GetFileByIdQuery, GetFileByIdQueryResult> handler)
    {
        var result = await handler.Handle(new GetFileByIdQuery { Id = fileId });
        return Results.Ok(result);
    }
}
