using System.Reflection;
using TwoDrive.Endpoints;

namespace TwoDrive.Common;

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
        //var createFileEndpoint = app.Services.GetServices<IEndpoint>().FirstOrDefault(endpoint => endpoint is CreateFileEndpoint);
        //createFileEndpoint?.MapEndpoint(app);




        var endpoints = app.Services.GetServices<IEndpoint>();
        foreach (var endpoint in endpoints)
        {
            endpoint.MapEndpoint(app);
        }
        return app;
    }
}