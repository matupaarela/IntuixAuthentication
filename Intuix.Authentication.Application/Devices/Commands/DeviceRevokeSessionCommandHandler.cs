using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Application.Common.Interfaces;
using MediatR;

namespace Intuix.Authentication.Application.Devices.Commands;

public class DeviceRevokeSessionCommandHandler : IRequestHandler<DeviceRevokeSessionCommand, Unit>
{
    private readonly IRefreshTokenRepository _repo;
    private readonly ICurrentUser _currentUser;

    public DeviceRevokeSessionCommandHandler(IRefreshTokenRepository repo, ICurrentUser currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeviceRevokeSessionCommand request, CancellationToken cancellationToken)
    {
        await _repo.RevokeSessionAsync(request.TokenId, _currentUser.UserId);
        return Unit.Value;
    }
}
