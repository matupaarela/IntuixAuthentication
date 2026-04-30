using Intuix.Authentication.Domain.Entities;

namespace Intuix.Authentication.Application.Auth.Interfaces
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetByCodeAsync(string code);
    }
}
