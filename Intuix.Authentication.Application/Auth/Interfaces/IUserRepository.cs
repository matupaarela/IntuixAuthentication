using Intuix.Authentication.Domain.Entities;

namespace Intuix.Authentication.Application.Auth.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);
    Task<List<string>> GetRolesAsync(Guid userId);
    Task<List<string>> GetPermissionsAsync(Guid userId);
    Task<Guid?> GetDefaultCompanyAsync(Guid userId);
    Task<List<Guid>> GetUserCompaniesAsync(Guid userId);
    Task<User?> GetByIdAsync(Guid userId);
}
