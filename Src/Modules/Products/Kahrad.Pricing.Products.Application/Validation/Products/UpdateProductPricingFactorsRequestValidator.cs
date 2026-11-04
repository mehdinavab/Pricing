using FluentValidation;
using Kahrad.Pricing.Products.Application.DTOs.Products;

namespace Kahrad.Pricing.Products.Application.Validation.Products;

public sealed class UpdateProductPricingFactorsRequestValidator : AbstractValidator<UpdateProductPricingFactorsRequest>
{
    public UpdateProductPricingFactorsRequestValidator()
    {
        RuleFor(x => x.BeautyFactorPercentage)
            .InclusiveBetween(-100m, 1000m).WithMessage("Beauty factor percentage must be between -100% and 1000%.");

        RuleFor(x => x.CommercialFactorPercentage)
            .InclusiveBetween(-100m, 1000m).WithMessage("Commercial factor percentage must be between -100% and 1000%.");

        RuleFor(x => x.SalesFactorPercentage)
            .InclusiveBetween(-100m, 1000m).WithMessage("Sales factor percentage must be between -100% and 1000%.");
    }
}
