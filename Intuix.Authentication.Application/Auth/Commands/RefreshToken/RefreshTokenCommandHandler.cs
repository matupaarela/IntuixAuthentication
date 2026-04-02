using Intuix.Authentication.Application.Auth.DTOs;
using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Domain.Interfaces;
using MediatR;
using System.Security.Cryptography;
using System.Text;
namespace Intuix.Authentication.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IUserRepository _userRepo;
    private readonly IJwtProvider _jwtProvider;
    private readonly IRefreshTokenService _refreshService;

    public RefreshTokenCommandHandler(
        IRefreshTokenRepository refreshRepo,
        IUserRepository userRepo,
        IJwtProvider jwtProvider,
        IRefreshTokenService refreshService)
    {
        _refreshRepo = refreshRepo;
        _userRepo = userRepo;
        _jwtProvider = jwtProvider;
        _refreshService = refreshService;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Hash del token recibido
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(request.RefreshToken));

        var existingToken = await _refreshRepo.GetByHashAsync(hash);

        if (existingToken == null)
            throw new Exception("Invalid refresh token");

        // 2. Validaciones críticas
        if (existingToken.RevokedAt != null)
        {
            // 🔥 TOKEN REUTILIZADO → ataque
            // aquí podrías revocar toda la cadena (opcional)
            throw new Exception("Token already revoked (possible reuse attack)");
        }

        if (existingToken.ExpiresAt < DateTime.UtcNow)
            throw new Exception("Token expired");

        var user = existingToken.User;

        if (user == null || !user.IsActive)
            throw new Exception("Invalid user");

        // 3. Obtener contexto
        var companyId = await _userRepo.GetDefaultCompanyAsync(user.Id)
            ?? throw new Exception("User has no company");

        var roles = await _userRepo.GetRolesAsync(user.Id);
        var permissions = await _userRepo.GetPermissionsAsync(user.Id);

        // 4. ROTACIÓN
        var (newToken, newHash) = _refreshService.Generate();

        // revocar actual
        existingToken.RevokedAt = DateTime.UtcNow;

        var newRefresh = new Domain.Entities.RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = newHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            ReplacedByToken = null
        };

        existingToken.ReplacedByToken = newRefresh.Id;

        await _refreshRepo.AddAsync(newRefresh);
        await _refreshRepo.SaveChangesAsync();

        // 5. Nuevo JWT
        var accessToken = _jwtProvider.GenerateToken(user, companyId, roles, permissions);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = newToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),

            UserId = user.Id,
            TenantId = user.TenantId,
            CompanyId = companyId
        };
    }
}
