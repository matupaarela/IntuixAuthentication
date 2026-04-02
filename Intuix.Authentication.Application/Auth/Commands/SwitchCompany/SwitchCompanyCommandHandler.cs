using Intuix.Authentication.Application.Auth.DTOs;
using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Domain.Interfaces;
using MediatR;

namespace Intuix.Authentication.Application.Auth.Commands.SwitchCompany;

public class SwitchCompanyCommandHandler
: IRequestHandler<SwitchCompanyCommand, AuthResponse>
{
    private readonly IUserRepository _userRepo;
    private readonly IJwtProvider _jwtProvider;
    private readonly ICurrentUser _currentUser;

    public SwitchCompanyCommandHandler(
        IUserRepository userRepo,
        IJwtProvider jwtProvider,
        ICurrentUser currentUser)
    {
        _userRepo = userRepo;
        _jwtProvider = jwtProvider;
        _currentUser = currentUser;
    }

    public async Task<AuthResponse> Handle(SwitchCompanyCommand request, CancellationToken cancellationToken)
    {
        // validar que la empresa pertenece al usuario
        var companies = await _userRepo.GetUserCompaniesAsync(_currentUser.UserId);

        if (!companies.Contains(request.CompanyId))
            throw new Exception("Unauthorized company");

        var roles = await _userRepo.GetRolesAsync(_currentUser.UserId);
        var permissions = await _userRepo.GetPermissionsAsync(_currentUser.UserId);

        var user = await _userRepo.GetByIdAsync(_currentUser.UserId)
            ?? throw new Exception("User not found");

        var token = _jwtProvider.GenerateToken(user, request.CompanyId, roles, permissions);

        return new AuthResponse
        {
            AccessToken = token,
            RefreshToken = string.Empty, // no rotamos aquí
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),

            UserId = user.Id,
            TenantId = user.TenantId,
            CompanyId = request.CompanyId
        };
    }
}
