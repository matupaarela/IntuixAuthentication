using MediatR;

namespace Intuix.Authentication.Application.Auth.Commands.Logout;

public record LogoutAllCommand() : IRequest<Unit>;
