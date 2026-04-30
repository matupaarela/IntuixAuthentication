using Intuix.Authentication.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Intuix.Authentication.Infrastructure.Security;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Guid? _tenantIdOverride;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId =>
        GetGuidClaim("sub");

    public Guid TenantId =>
        GetGuidClaim("tenant") != Guid.Empty
            ? GetGuidClaim("tenant")
            : _tenantIdOverride ?? Guid.Empty;

    public Guid CompanyId =>
        GetGuidClaim("company");

    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public void SetTenant(Guid tenantId)
    {
        _tenantIdOverride = tenantId;
    }

    private Guid GetGuidClaim(string type)
    {
        var value = _httpContextAccessor.HttpContext?
            .User?
            .FindFirst(type)?.Value;

        return value != null ? Guid.Parse(value) : Guid.Empty;
    }
}
