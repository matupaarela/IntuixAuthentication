using FluentValidation;
using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Application.Devices.Commands;

namespace Intuix.Authentication.Application.Devices.Validators;

public sealed class DeviceRevokeAllSessionsCommandValidator : AbstractValidator<DeviceRevokeAllSessionsCommand>
{
    public DeviceRevokeAllSessionsCommandValidator(ICurrentUser currentUser)
    {
        RuleFor(x => x).Custom((_, context) =>
        {
            if (currentUser.UserId == Guid.Empty)
                context.AddFailure("Security validation failed.");
        });
    }
}
