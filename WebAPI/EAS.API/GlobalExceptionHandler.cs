using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;

namespace EAS.API;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var traceId = Activity.Current?.Id ?? httpContext.TraceIdentifier;

        logger.LogError(exception, "Could not process a request on {MachineName}. TraceId : {TraceId}", Environment.MachineName, traceId);

        var (statusCode, title) = MapException(exception);

        await Results.Problem(
            title: title,
            statusCode: statusCode,
            extensions: new Dictionary<string, object?>
            {
                { "traceId", traceId },
                { "machineName", Environment.MachineName }
            }
        ).ExecuteAsync(httpContext);

        return true;
    }

    private static (int statusCode, string title) MapException(Exception exception)
    {
        return exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, exception.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "You are not authorized to access this resource."),
            //NotFoundException => (StatusCodes.Status404NotFound, "The requested resource was not found."),
            _ => (StatusCodes.Status500InternalServerError, "We made a mistake but we're working on it!")
        };
    }
}
