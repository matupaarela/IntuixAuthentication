using MediatR;

namespace Intuix.Authentication.Application.Devices.Commands;

public record DeviceRevokeAllSessionsCommand() : IRequest<Unit>;
