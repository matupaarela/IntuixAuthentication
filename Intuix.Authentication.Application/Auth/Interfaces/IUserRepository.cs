using Intuix.Authentication.Domain.Entities;

namespace Intuix.Authentication.Application.Auth.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<List<string>> GetRolesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<string>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Guid?> GetDefaultCompanyAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Guid>> GetUserCompaniesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<User?> GetByIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
