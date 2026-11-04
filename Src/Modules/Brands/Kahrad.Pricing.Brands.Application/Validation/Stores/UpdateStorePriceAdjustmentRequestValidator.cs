using FluentValidation;
using Kahrad.Pricing.Brands.Application.DTOs.Stores;

namespace Kahrad.Pricing.Brands.Application.Validation.Stores;

public sealed class UpdateStorePriceAdjustmentRequestValidator : AbstractValidator<UpdateStorePriceAdjustmentRequest>
{
    public UpdateStorePriceAdjustmentRequestValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Percentage)
            .InclusiveBetween(0m, 100m)
            .WithMessage("Price adjustment percentage must be between 0 and 100.");
    }
}
