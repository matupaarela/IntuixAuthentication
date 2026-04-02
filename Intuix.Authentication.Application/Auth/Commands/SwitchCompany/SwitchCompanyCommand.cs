using Intuix.Authentication.Application.Auth.DTOs;
using MediatR;

namespace Intuix.Authentication.Application.Auth.Commands.SwitchCompany;

public record SwitchCompanyCommand(Guid CompanyId) : IRequest<AuthResponse>;
