using MediatR;

namespace Intuix.Authentication.Application.Devices.Commands;

public record DeviceRevokeSessionCommand(Guid TokenId) : IRequest<Unit>;
