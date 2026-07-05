using FluentValidation;
using Intuix.Authentication.Application.Auth.Commands.RefreshToken;

namespace Intuix.Authentication.Application.Auth.Validators;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
