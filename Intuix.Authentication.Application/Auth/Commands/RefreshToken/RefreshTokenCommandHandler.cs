using Intuix.Authentication.Application.Auth.DTOs;
using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Domain.Interfaces;
using MediatR;

namespace Intuix.Authentication.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IUserRepository _userRepo;
    private readonly IJwtProvider _jwtProvider;
    private readonly IRefreshTokenService _refreshService;
    private readonly ITenantContext _tenantContext;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshRepo,
        IUserRepository userRepo,
        IJwtProvider jwtProvider,
        IRefreshTokenService refreshService,
        ITenantContext tenantContext)
    {
        _refreshRepo = refreshRepo;
        _userRepo = userRepo;
        _jwtProvider = jwtProvider;
        _refreshService = refreshService;
        _tenantContext = tenantContext;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var hash = _refreshService.Hash(request.RefreshToken);

        var existingToken = await _refreshRepo.GetByHashAsync(hash, cancellationToken);

        if (existingToken == null || existingToken.User == null || !existingToken.User.IsActive || existingToken.User.IsLocked)
            throw new InvalidOperationException("Security validation failed.");

        if (existingToken.User.TenantId == Guid.Empty)
            throw new InvalidOperationException("Security validation failed.");

        _tenantContext.SetTenant(existingToken.User.TenantId);

        if (existingToken.RevokedAt != null)
        {
            await _refreshRepo.RevokeTokenChainAsync(
                existingToken.Id,
                "Refresh token reused",
                now,
                cancellationToken);

            throw new InvalidOperationException("Security validation failed.");
        }

        if (existingToken.ExpiresAt <= now)
            throw new InvalidOperationException("Security validation failed.");

        var user = existingToken.User;

        var companyId = await _userRepo.GetDefaultCompanyAsync(user.Id, cancellationToken)
            ?? throw new InvalidOperationException("Security validation failed.");

        var roles = await _userRepo.GetRolesAsync(user.Id, cancellationToken);
        var permissions = await _userRepo.GetPermissionsAsync(user.Id, cancellationToken);

        var (newToken, newHash) = _refreshService.Generate();
        var newRefreshId = Guid.NewGuid();

        existingToken.RevokedAt = now;
        existingToken.ReplacedByToken = newRefreshId;
        existingToken.RevocationReason = "Refresh token rotated";
        existingToken.LastUsedAt = now;

        var newRefresh = new Domain.Entities.RefreshToken
        {
            Id = newRefreshId,
            UserId = user.Id,
            TokenHash = newHash,
            ExpiresAt = now.AddDays(7),
            CreatedAt = now,
            LastUsedAt = now,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            Device = request.UserAgent
        };

        await _refreshRepo.AddAsync(newRefresh, cancellationToken);

        var accessToken = _jwtProvider.GenerateToken(user, companyId, newRefresh.Id, roles, permissions);

        await _refreshRepo.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newToken,
            ExpiresAt = now.AddMinutes(15),

            UserId = user.Id,
            TenantId = user.TenantId,
            CompanyId = companyId
        };
    }
}
