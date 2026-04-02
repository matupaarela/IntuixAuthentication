using Intuix.Authentication.Application.Auth.DTOs;
using MediatR;

namespace Intuix.Authentication.Application.Auth.Commands.Login;

public record LoginCommand(string Username, string Password) : IRequest<AuthResponse>;
