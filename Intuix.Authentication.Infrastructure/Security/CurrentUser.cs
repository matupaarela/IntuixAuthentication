using Intuix.Authentication.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Intuix.Authentication.Infrastructure.Security;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId =>
        GetGuidClaim("sub");

    public Guid TenantId =>
        GetGuidClaim("tenant");

    public Guid CompanyId =>
        GetGuidClaim("company");

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    private Guid GetGuidClaim(string type)
    {
        var value = _httpContextAccessor.HttpContext?
            .User?
            .FindFirst(type)?.Value;

        return value != null ? Guid.Parse(value) : Guid.Empty;
    }
}
