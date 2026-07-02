using Intuix.Authentication.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Intuix.Authentication.Api.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ICurrentUser currentUser)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            if (currentUser.UserId == Guid.Empty
                || currentUser.TenantId == Guid.Empty
                || currentUser.CompanyId == Guid.Empty)
            {
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
