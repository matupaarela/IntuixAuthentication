using Intuix.Authentication.Application.Common.Interfaces;

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
            if (currentUser.TenantId == Guid.Empty)
                throw new Exception("Invalid tenant context");

            if (currentUser.CompanyId == Guid.Empty)
                throw new Exception("Invalid company context");
        }

        await _next(context);
    }
}
