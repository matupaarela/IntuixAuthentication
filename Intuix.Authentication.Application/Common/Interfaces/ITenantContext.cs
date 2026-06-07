namespace Intuix.Authentication.Application.Common.Interfaces;

public interface ITenantContext
{
    Guid TenantId { get; }

    void SetTenant(Guid tenantId);
}
