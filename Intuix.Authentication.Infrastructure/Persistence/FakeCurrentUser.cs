using Intuix.Authentication.Application.Common.Interfaces;

namespace Intuix.Authentication.Infrastructure.Persistence;

public class FakeCurrentUser : ICurrentUser
{
    public Guid UserId => Guid.Empty;
    public Guid TenantId => Guid.Empty;
    public Guid CompanyId => Guid.Empty;
    public bool IsAuthenticated => false;
}
