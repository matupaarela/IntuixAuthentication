using Intuix.Authentication.Domain.Entities;

namespace Intuix.Authentication.Application.Auth.Interfaces;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetByHashAsync(byte[] hash, CancellationToken cancellationToken = default);
    Task<List<RefreshToken>> GetActiveSessionsByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task RevokeSessionAsync(Guid tokenId, Guid userId, CancellationToken cancellationToken = default);
    Task RevokeAllSessionsExceptCurrentAsync(Guid userId, Guid currentTokenId, CancellationToken cancellationToken = default);
    Task RevokeTokenChainAsync(Guid tokenId, string revocationReason, DateTime revokedAt, CancellationToken cancellationToken = default);
    Task RevokeAllUserTokensAsync(Guid userId, string revocationReason, DateTime revokedAt, CancellationToken cancellationToken = default);
}
