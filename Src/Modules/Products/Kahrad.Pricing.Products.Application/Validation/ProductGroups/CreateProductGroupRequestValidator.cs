using FluentValidation;
using Kahrad.Pricing.Products.Application.DTOs.ProductGroups;

namespace Kahrad.Pricing.Products.Application.Validation.ProductGroups;

public sealed class CreateProductGroupRequestValidator : AbstractValidator<CreateProductGroupRequest>
{
    public CreateProductGroupRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Group name is required.")
            .MaximumLength(150).WithMessage("Group name cannot exceed 150 characters.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Group code is required.")
            .MaximumLength(50).WithMessage("Group code cannot exceed 50 characters.")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage("Group code can only contain letters, numbers, underscores and hyphens.");

        RuleFor(x => x.PriceEffectPercentage)
            .InclusiveBetween(-100m, 1000m).WithMessage("Price effect percentage must be between -100% and 1000%.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}
