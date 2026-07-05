using Intuix.Authentication.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;

namespace Intuix.Authentication.Api.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantMiddleware> _logger;

    public TenantMiddleware(RequestDelegate next, ILogger<TenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context, ICurrentUser currentUser)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            if (currentUser.UserId == Guid.Empty
                || currentUser.TenantId == Guid.Empty
                || currentUser.CompanyId == Guid.Empty)
            {
                _logger.LogWarning(
                    "Tenant guard failed for {Path} tenant {TenantId} user {UserId} session {SessionId}",
                    context.Request.Path,
                    currentUser.TenantId,
                    currentUser.UserId,
                    currentUser.RefreshTokenId);

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Authorization failed.",
                    Detail = "Authorization failed.",
                    Instance = context.Request.Path
                });

                return;
            }
        }

        await _next(context);
    }
}
