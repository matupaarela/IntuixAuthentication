using Intuix.Authentication.Domain.Entities;

namespace Intuix.Authentication.Application.Auth.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task SaveChangesAsync();
    Task<RefreshToken?> GetByHashAsync(byte[] hash);
    Task<List<RefreshToken>> GetActiveSessionsByUserAsync(Guid userId);
    Task RevokeSessionAsync(Guid tokenId, Guid userId);
    Task RevokeAllSessionsExceptCurrentAsync(Guid userId, Guid currentTokenId);
    Task RevokeTokenChainAsync(Guid tokenId, string revocationReason, DateTime revokedAt);
    Task RevokeAllUserTokensAsync(Guid userId, string revocationReason, DateTime revokedAt);
}
