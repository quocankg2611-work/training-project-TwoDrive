using System.Diagnostics;

namespace TwoDrive.Api.Middlewares;

public sealed class LoggingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, ILogger<LoggingMiddleware> logger)
    {
        using var _ = logger.BeginScope(new Dictionary<string, object?>
        {
            ["RequestHost"] = context.Request.Host.Value,
            ["RequestScheme"] = context.Request.Scheme,
            ["TraceIdentifier"] = context.TraceIdentifier,
            ["EndpointName"] = context.GetEndpoint()?.DisplayName
        });

        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();
        var statusCode = context.Response.StatusCode;
        var level = statusCode >= StatusCodes.Status500InternalServerError
            ? LogLevel.Error
            : statusCode >= StatusCodes.Status400BadRequest
                ? LogLevel.Warning
                : LogLevel.Information;

        logger.Log(
            level,
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms",
            context.Request.Method,
            context.Request.Path,
            statusCode,
            stopwatch.Elapsed.TotalMilliseconds);
    }
}
