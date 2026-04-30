namespace Intuix.Authentication.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    Guid TenantId { get; }
    Guid CompanyId { get; }

    bool IsAuthenticated { get; }

    void SetTenant(Guid tenantId);
}
