using Intuix.Authentication.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private Guid? _tenantIdOverride;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid UserId =>
        GetGuidClaim("sub") != Guid.Empty
            ? GetGuidClaim("sub")
            : GetGuidClaim(ClaimTypes.NameIdentifier);

    public Guid TenantId
    {
        get
        {
            var tenant = GetGuidClaim("tenant");

            if (tenant != Guid.Empty)
                return tenant;

            return _tenantIdOverride ?? Guid.Empty;
        }
    }

    public Guid CompanyId =>
        GetGuidClaim("company");

    public bool IsAuthenticated =>
        User?.Identity?.IsAuthenticated ?? false;

    public void SetTenant(Guid tenantId)
    {
        _tenantIdOverride = tenantId;
    }

    private Guid GetGuidClaim(string type)
    {
        var value = User?.FindFirst(type)?.Value;

        return Guid.TryParse(value, out var guid)
            ? guid
            : Guid.Empty;
    }
}