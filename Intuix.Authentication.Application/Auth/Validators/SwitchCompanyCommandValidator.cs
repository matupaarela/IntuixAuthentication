using FluentValidation;
using Intuix.Authentication.Application.Auth.Commands.SwitchCompany;

namespace Intuix.Authentication.Application.Auth.Validators;

public sealed class SwitchCompanyCommandValidator : AbstractValidator<SwitchCompanyCommand>
{
    public SwitchCompanyCommandValidator()
    {
        RuleFor(x => x.CompanyId)
            .NotEmpty();
    }
}
