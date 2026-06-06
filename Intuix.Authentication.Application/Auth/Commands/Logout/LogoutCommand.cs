using MediatR;

namespace Intuix.Authentication.Application.Auth.Commands.Logout
{
    public record LogoutCommand(string RefreshToken) : IRequest<Unit>;
}
