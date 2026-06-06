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

    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?> GetByHashAsync(byte[] hash)
    {
        return await _context.RefreshTokens
            .Include(x => x.User)
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.TokenHash == hash);
    }

    public async Task RevokeTokenChainAsync(Guid tokenId, string revocationReason, DateTime revokedAt)
    {
        var current = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Id == tokenId);
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

            current = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Id == nextTokenId);
        }

        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllUserTokensAsync(Guid userId, string revocationReason, DateTime revokedAt)
    {
        var tokens = await _context.RefreshTokens
            .Where(x => x.UserId == userId && x.RevokedAt == null)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.RevokedAt = revokedAt;
            token.RevocationReason = string.IsNullOrWhiteSpace(token.RevocationReason)
                ? revocationReason
                : token.RevocationReason;
        }

        await _context.SaveChangesAsync();
    }
}
