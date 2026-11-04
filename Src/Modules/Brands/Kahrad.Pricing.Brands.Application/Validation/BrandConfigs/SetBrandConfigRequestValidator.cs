using FluentValidation;
using Kahrad.Pricing.Brands.Application.DTOs.BrandConfigs;

namespace Kahrad.Pricing.Brands.Application.Validation.BrandConfigs;

public sealed class SetBrandConfigRequestValidator : AbstractValidator<SetBrandConfigRequest>
{
    public SetBrandConfigRequestValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.DisplayName)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Value)
            .NotNull()
            .MaximumLength(2000);

        RuleFor(x => x.Category)
            .MaximumLength(100);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
