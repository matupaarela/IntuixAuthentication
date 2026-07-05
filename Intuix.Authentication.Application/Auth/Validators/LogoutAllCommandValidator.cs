using FluentValidation;
using Intuix.Authentication.Application.Auth.Commands.Logout;
using Intuix.Authentication.Application.Common.Interfaces;

namespace Intuix.Authentication.Application.Auth.Validators;

public sealed class LogoutAllCommandValidator : AbstractValidator<LogoutAllCommand>
{
    public LogoutAllCommandValidator(ICurrentUser currentUser)
    {
        RuleFor(x => x).Custom((_, context) =>
        {
            if (currentUser.UserId == Guid.Empty)
                context.AddFailure("Security validation failed.");
        });
    }
}
