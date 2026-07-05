using FluentValidation;
using Intuix.Authentication.Application.Auth.Commands.Login;

namespace Intuix.Authentication.Application.Auth.Validators;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty();

        RuleFor(x => x.Password)
            .NotEmpty();

        RuleFor(x => x.TenantCode)
            .NotEmpty()
            .MaximumLength(50);
    }
}
