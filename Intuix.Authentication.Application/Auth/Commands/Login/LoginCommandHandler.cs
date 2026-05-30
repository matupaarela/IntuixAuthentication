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
    private readonly ICurrentUser _currentUser;

    public LoginCommandHandler(
        IUserRepository userRepo,
        IRefreshTokenRepository refreshRepo,
        IPasswordHasher hasher,
        IJwtProvider jwtProvider,
        IRefreshTokenService refreshService,
        ITenantRepository tenantRepository,
        ICurrentUser currentUser)
    {
        _userRepo = userRepo;
        _refreshRepo = refreshRepo;
        _hasher = hasher;
        _jwtProvider = jwtProvider;
        _refreshService = refreshService;
        _tenantRepo = tenantRepository;
        _currentUser = currentUser;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // 🔹 0. Resolver tenant
        var tenant = await _tenantRepo.GetByCodeAsync(request.TenantCode);

        if (tenant == null)
            throw new Exception("Invalid tenant");

        // 🔹 1. Setear tenant en contexto (CRÍTICO)
        _currentUser.SetTenant(tenant.Id);

        // 🔹 2. Buscar usuario (ahora sí funciona el QueryFilter)
        var user = await _userRepo.GetByUsernameAsync(request.Username);

        if (user == null || !user.IsActive)
            throw new Exception("Invalid credentials");

        if (user.IsLocked)
            throw new Exception("User locked");

        //var pass = _hasher.Hash(request.Password);
        //var passToDB = Convert.ToBase64String(pass);

        // 🔹 3. Validar password
        var isValid = _hasher.Verify(
            request.Password,
            Convert.FromBase64String(user.PasswordHash)
        );

        if (!isValid)
        {
            user.FailedAttempts++;

            if (user.FailedAttempts >= 5)
                user.IsLocked = true;

            throw new Exception("Invalid credentials");
        }

        user.FailedAttempts = 0;
        user.LastLogin = DateTime.UtcNow;

        var companyId = await _userRepo.GetDefaultCompanyAsync(user.Id);

        if (companyId == null)
            throw new Exception("User has no company assigned");

        var roles = await _userRepo.GetRolesAsync(user.Id);
        var permissions = await _userRepo.GetPermissionsAsync(user.Id);

        var accessToken = _jwtProvider.GenerateToken(user, companyId.Value, roles, permissions);

        var (refreshToken, hash) = _refreshService.Generate();

        var refreshEntity = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = hash,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _refreshRepo.AddAsync(refreshEntity);
        await _refreshRepo.SaveChangesAsync();

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),

            UserId = user.Id,
            TenantId = user.TenantId,
            CompanyId = companyId.Value
        };
    }
}
