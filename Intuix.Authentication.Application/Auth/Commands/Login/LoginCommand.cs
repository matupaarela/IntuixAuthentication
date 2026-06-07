using Intuix.Authentication.Application.Auth.DTOs;
using MediatR;

namespace Intuix.Authentication.Application.Auth.Commands.Login;

public record LoginCommand(
    string Username,
    string Password,
    string TenantCode,
    string? IpAddress = null,
    string? UserAgent = null) : IRequest<AuthResponse>;
