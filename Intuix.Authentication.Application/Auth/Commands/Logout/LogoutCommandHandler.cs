using Intuix.Authentication.Application.Auth.Interfaces;
using MediatR;

namespace Intuix.Authentication.Application.Auth.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IRefreshTokenRepository _repo;

        public LogoutCommandHandler(IRefreshTokenRepository repo)
        {
            _repo = repo;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            await _repo.RevokeAllUserTokensAsync(
                request.UserId,
                "User logout"
            );
        }
    }
}
