using FluentValidation;
using Intuix.Authentication.Application.Common.Interfaces;
using Intuix.Authentication.Application.Devices.Queries;

namespace Intuix.Authentication.Application.Devices.Validators;

public sealed class DeviceGetListQueryValidator : AbstractValidator<DeviceGetListQuery>
{
    public DeviceGetListQueryValidator(ICurrentUser currentUser)
    {
        RuleFor(x => x).Custom((_, context) =>
        {
            if (currentUser.UserId == Guid.Empty)
                context.AddFailure("Security validation failed.");
        });
    }
}
