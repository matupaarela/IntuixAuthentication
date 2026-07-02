using Intuix.Authentication.Application.Auth.Interfaces;
using Intuix.Authentication.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Intuix.Authentication.Application.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly IRefreshTokenRepository _repo;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ILogger<LogoutCommandHandler> _logger;

        public LogoutCommandHandler(
            IRefreshTokenRepository repo,
            IRefreshTokenService refreshTokenService,
            ILogger<LogoutCommandHandler> logger)
        {
            _repo = repo;
            _refreshTokenService = refreshTokenService;
            _logger = logger;
        }

        public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var hash = _refreshTokenService.Hash(request.RefreshToken);
            var token = await _repo.GetByHashAsync(hash, cancellationToken);

            if (token == null)
            {
                _logger.LogInformation("Logout requested for unknown refresh token");
                return Unit.Value;
            }

            await _repo.RevokeTokenChainAsync(
                token.Id,
                "User logout",
                DateTime.UtcNow,
                cancellationToken);

            _logger.LogInformation(
                "Logout revoked refresh token chain for user {UserId}",
                token.UserId);

            return Unit.Value;
        }
    }
}
