using FluentValidation;
using Kahrad.Pricing.Products.Application.DTOs.Products;

namespace Kahrad.Pricing.Products.Application.Validation.Products;

public sealed class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(250).WithMessage("Product name cannot exceed 250 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Product code is required.")
            .MaximumLength(100).WithMessage("Product code cannot exceed 100 characters.");

        RuleFor(x => x.GroupId)
            .GreaterThan(0).WithMessage("Valid group ID is required.");

        RuleFor(x => x.BasePrice)
            .GreaterThanOrEqualTo(0).WithMessage("Base price cannot be negative.");

        RuleFor(x => x.BeautyFactorPercentage)
            .InclusiveBetween(-100m, 1000m).WithMessage("Beauty factor percentage must be between -100% and 1000%.");

        RuleFor(x => x.CommercialFactorPercentage)
            .InclusiveBetween(-100m, 1000m).WithMessage("Commercial factor percentage must be between -100% and 1000%.");

        RuleFor(x => x.SalesFactorPercentage)
            .InclusiveBetween(-100m, 1000m).WithMessage("Sales factor percentage must be between -100% and 1000%.");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
    }
}
