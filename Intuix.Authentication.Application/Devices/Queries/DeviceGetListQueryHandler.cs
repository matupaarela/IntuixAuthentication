using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Application.Devices.DTOs;
using Intuix.Authentication.Domain.Entities;
using MediatR;

namespace Intuix.Authentication.Application.Devices.Queries;

public class DeviceGetListQueryHandler : IRequestHandler<DeviceGetListQuery, List<DeviceSessionResponse>>
{
    private readonly IRefreshTokenRepository _repo;
    private readonly ICurrentUser _currentUser;

    public DeviceGetListQueryHandler(IRefreshTokenRepository repo, ICurrentUser currentUser)
    {
        _repo = repo;
        _currentUser = currentUser;
    }

    public async Task<List<DeviceSessionResponse>> Handle(DeviceGetListQuery request, CancellationToken cancellationToken)
    {
        var sessions = await _repo.GetActiveSessionsByUserAsync(_currentUser.UserId, cancellationToken);
        var currentTokenId = ResolveCurrentTokenId(sessions);

        return sessions
            .Select(x => new DeviceSessionResponse
            {
                TokenId = x.Id,
                IpAddress = x.IpAddress,
                UserAgent = x.UserAgent,
                CreatedAt = x.CreatedAt,
                LastUsedAt = x.LastUsedAt == default ? x.CreatedAt : x.LastUsedAt,
                IsCurrent = x.Id == currentTokenId
            })
            .ToList();
    }

    private Guid ResolveCurrentTokenId(List<RefreshToken> sessions)
    {
        if (_currentUser.RefreshTokenId != Guid.Empty)
            return _currentUser.RefreshTokenId;

        return sessions
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => x.Id)
            .FirstOrDefault();
    }
}
