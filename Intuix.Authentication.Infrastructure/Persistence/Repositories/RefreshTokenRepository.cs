using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Intuix.Authentication.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AuthDbContext _context;

    public RefreshTokenRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken token, CancellationToken cancellationToken = default)
    {
        await _context.RefreshTokens.AddAsync(token, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetByHashAsync(byte[] hash, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.TokenHash == hash, cancellationToken);
    }

    public async Task<List<RefreshToken>> GetActiveSessionsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .AsNoTracking()
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .OrderByDescending(x => x.LastUsedAt)
            .ThenByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task RevokeSessionAsync(Guid tokenId, Guid userId, CancellationToken cancellationToken = default)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Id == tokenId && x.UserId == userId, cancellationToken);

        if (token == null)
            return;

        token.RevokedAt = DateTime.UtcNow;
        token.RevocationReason = "Session revoked";

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllSessionsExceptCurrentAsync(Guid userId, Guid currentTokenId, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null && x.Id != currentTokenId)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.RevocationReason = "Revoke all sessions except current";
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeTokenChainAsync(Guid tokenId, string revocationReason, DateTime revokedAt, CancellationToken cancellationToken = default)
    {
        var current = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Id == tokenId, cancellationToken);
        if (current == null)
            return;

        var visited = new HashSet<Guid>();

        while (current != null && visited.Add(current.Id))
        {
            if (current.RevokedAt == null)
                current.RevokedAt = revokedAt;

            if (string.IsNullOrWhiteSpace(current.RevocationReason))
                current.RevocationReason = revocationReason;

            if (current.ReplacedByToken is not Guid nextTokenId)
                break;

            current = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Id == nextTokenId, cancellationToken);
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, string revocationReason, DateTime revokedAt, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.RevokedAt = revokedAt;
            token.RevocationReason = string.IsNullOrWhiteSpace(token.RevocationReason)
                ? revocationReason
                : token.RevocationReason;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
