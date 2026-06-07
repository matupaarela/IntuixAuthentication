using Intuix.Authentication.Application.Auth.DTOs;
using MediatR;

namespace Intuix.Authentication.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
    string RefreshToken,
    string? IpAddress = null,
    string? UserAgent = null) : IRequest<AuthResponse>;
