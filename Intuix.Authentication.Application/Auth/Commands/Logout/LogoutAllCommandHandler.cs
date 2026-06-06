using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Intuix.Authentication.Application.Auth.Commands.Logout;

public class LogoutAllCommandHandler : IRequestHandler<LogoutAllCommand, Unit>
{
    private readonly IRefreshTokenRepository _repo;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<LogoutAllCommandHandler> _logger;

    public LogoutAllCommandHandler(
        IRefreshTokenRepository repo,
        ICurrentUser currentUser,
        ILogger<LogoutAllCommandHandler> logger)
    {
        _repo = repo;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Unit> Handle(LogoutAllCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId == Guid.Empty)
            throw new InvalidOperationException("Current user context is required");

        await _repo.RevokeAllUserTokensAsync(
            _currentUser.UserId,
            "Logout all",
            DateTime.UtcNow);

        _logger.LogInformation(
            "Logout-all revoked all refresh tokens for user {UserId}",
            _currentUser.UserId);

        return Unit.Value;
    }
}
