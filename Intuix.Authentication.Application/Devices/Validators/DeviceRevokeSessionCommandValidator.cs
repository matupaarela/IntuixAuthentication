using FluentValidation;
using Intuix.Authentication.Application.Devices.Commands;

namespace Intuix.Authentication.Application.Devices.Validators;

public sealed class DeviceRevokeSessionCommandValidator : AbstractValidator<DeviceRevokeSessionCommand>
{
    public DeviceRevokeSessionCommandValidator()
    {
        RuleFor(x => x.TokenId)
            .NotEmpty();
    }
}
