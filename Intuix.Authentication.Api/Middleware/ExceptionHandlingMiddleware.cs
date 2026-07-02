using Microsoft.AspNetCore.Mvc;

namespace Intuix.Authentication.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            if (context.Response.HasStarted)
                throw;

            var (statusCode, title) = MapException(ex);

            _logger.LogWarning(ex, "Request failed with status {StatusCode} for {Path}", statusCode, context.Request.Path);

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            await context.Response.WriteAsJsonAsync(new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = title,
                Instance = context.Request.Path
            });
        }
    }

    private static (int StatusCode, string Title) MapException(Exception exception)
    {
        return exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Unauthorized company."),
            InvalidOperationException => (StatusCodes.Status401Unauthorized, "Security validation failed."),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
        };
    }
}
