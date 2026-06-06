using Intuix.Authentication.Application.Devices.DTOs;
using MediatR;

namespace Intuix.Authentication.Application.Devices.Queries;

public record DeviceGetListQuery() : IRequest<List<DeviceSessionResponse>>;
