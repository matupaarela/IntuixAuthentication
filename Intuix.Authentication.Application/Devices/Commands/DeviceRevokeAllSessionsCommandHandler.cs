using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Application.Common.Interfaces;
using MediatR;

namespace Intuix.Authentication.Application.Devices.Commands;

public class DeviceRevokeAllSessionsCommandHandler : IRequestHandler<DeviceRevokeAllSessionsCommand, Unit>
{
    private readonly IRefreshTokenRepository _repo;
    private readonly ICurrentUser _currentUser;

    public DeviceRevokeAllSessionsCommandHandler(IRefreshTokenRepository repo, ICurrentUser currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task<Unit> Handle(DeviceRevokeAllSessionsCommand request, CancellationToken cancellationToken)
    {
        var currentTokenId = _currentUser.RefreshTokenId;

        if (currentTokenId == Guid.Empty)
        {
            var sessions = await _repo.GetActiveSessionsByUserAsync(_currentUser.UserId);
            currentTokenId = sessions
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => x.Id)
                .FirstOrDefault();
        }

        await _repo.RevokeAllSessionsExceptCurrentAsync(_currentUser.UserId, currentTokenId);
        return Unit.Value;
    }
}
