using Intuix.Authentication.Domain.Entities;

namespace Intuix.Authentication.Application.Auth.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task SaveChangesAsync();
    Task<RefreshToken?> GetByHashAsync(byte[] hash);
}
