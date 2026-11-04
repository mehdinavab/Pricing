using FluentValidation;
using Kahrad.Pricing.Products.Application.DTOs.Features;

namespace Kahrad.Pricing.Products.Application.Validation.Features;

public sealed class AddProductFeatureRequestValidator : AbstractValidator<AddProductFeatureRequest>
{
    public AddProductFeatureRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Feature name is required.")
            .MaximumLength(150).WithMessage("Feature name cannot exceed 150 characters.");

        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Feature value is required.")
            .MaximumLength(500).WithMessage("Feature value cannot exceed 500 characters.");

        RuleFor(x => x.PriceEffectPercentage)
            .InclusiveBetween(-100m, 1000m).WithMessage("Price effect percentage must be between -100% and 1000%.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}
