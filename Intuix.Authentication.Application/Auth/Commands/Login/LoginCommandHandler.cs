using Intuix.Authentication.Application.Auth.DTOs;
using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Domain.Interfaces;
using MediatR;

namespace Intuix.Authentication.Application.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtProvider _jwtProvider;
    private readonly IRefreshTokenService _refreshService;

    private readonly ITenantRepository _tenantRepo;
    private readonly ITenantContext _tenantContext;

    public LoginCommandHandler(
        IUserRepository userRepo,
        IRefreshTokenRepository refreshRepo,
        IPasswordHasher hasher,
        IJwtProvider jwtProvider,
        IRefreshTokenService refreshService,
        ITenantRepository tenantRepository,
        ITenantContext tenantContext)
    {
        _userRepo = userRepo;
        _refreshRepo = refreshRepo;
        _hasher = hasher;
        _jwtProvider = jwtProvider;
        _refreshService = refreshService;
        _tenantRepo = tenantRepository;
        _tenantContext = tenantContext;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var tenant = await _tenantRepo.GetByCodeAsync(request.TenantCode, cancellationToken);

        if (tenant is null || !tenant.IsActive)
            throw new InvalidOperationException("Authentication failed.");

        _tenantContext.SetTenant(tenant.Id);

        var user = await _userRepo.GetByUsernameAsync(request.Username, cancellationToken);

        if (user is null || !user.IsActive || user.IsLocked)
            throw new InvalidOperationException("Authentication failed.");

        bool isValid;

        try
        {
            isValid = _hasher.Verify(request.Password, Convert.FromBase64String(user.PasswordHash));
        }
        catch (FormatException)
        {
            isValid = false;
        }

        if (!isValid)
        {
            user.FailedAttempts++;

            if (user.FailedAttempts >= 5)
                user.IsLocked = true;

            await _userRepo.SaveChangesAsync(cancellationToken);
            throw new InvalidOperationException("Authentication failed.");
        }

        var companyId = await _userRepo.GetDefaultCompanyAsync(user.Id, cancellationToken);

        if (companyId is null)
            throw new InvalidOperationException("Authentication failed.");

        user.FailedAttempts = 0;
        user.LastLogin = now;

        var roles = await _userRepo.GetRolesAsync(user.Id, cancellationToken);
        var permissions = await _userRepo.GetPermissionsAsync(user.Id, cancellationToken);

        var (refreshToken, hash) = _refreshService.Generate();

        var refreshEntity = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = hash,
            ExpiresAt = now.AddDays(7),
            CreatedAt = now,
            LastUsedAt = now,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent,
            Device = request.UserAgent
        };

        await _refreshRepo.AddAsync(refreshEntity);
        var accessToken = _jwtProvider.GenerateToken(user, companyId.Value, refreshEntity.Id, roles, permissions);

        await _userRepo.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = now.AddMinutes(15),

            UserId = user.Id,
            TenantId = user.TenantId,
            CompanyId = companyId.Value
        };
    }
}
