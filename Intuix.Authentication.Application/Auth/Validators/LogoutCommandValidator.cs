using FluentValidation;
using Intuix.Authentication.Application.Auth.Commands.Logout;

namespace Intuix.Authentication.Application.Auth.Validators;

public sealed class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
