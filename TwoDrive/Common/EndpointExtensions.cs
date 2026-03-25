using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using TwoDrive.Common;

namespace TwoDrive.Api.Common;

public static class EndpointExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var endpointTypes = assembly.DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } && typeof(IEndpoint).IsAssignableFrom(type))
            .Select(type => type.AsType());
        foreach (var endpointType in endpointTypes)
        {
            services.AddTransient(typeof(IEndpoint), endpointType);
        }
        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointsGroup = app.MapGroup(string.Empty)
            .WithMetadata(
                new ProducesResponseTypeAttribute(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest),
                new ProducesResponseTypeAttribute(typeof(ApiErrorResponse), StatusCodes.Status404NotFound),
                new ProducesResponseTypeAttribute(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError));

        var endpoints = app.Services.GetServices<IEndpoint>();
        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(endpointsGroup);
        }

        return app;
    }
}