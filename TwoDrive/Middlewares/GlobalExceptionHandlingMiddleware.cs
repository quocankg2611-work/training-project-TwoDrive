using TwoDrive.Api.Common;

namespace TwoDrive.Api.Middlewares;

public sealed class GlobalExceptionHandlingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, ILogger<GlobalExceptionHandlingMiddleware> logger)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception while processing request.");

            if (context.Response.HasStarted)
            {
                throw;
            }

            var (statusCode, message) = ex switch
            {
                KeyNotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                InvalidOperationException => (StatusCodes.Status400BadRequest, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new ApiErrorResponse(message));
        }
    }
}
