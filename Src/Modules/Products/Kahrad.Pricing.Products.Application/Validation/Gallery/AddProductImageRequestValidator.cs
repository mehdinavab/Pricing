using FluentValidation;
using Kahrad.Pricing.Products.Application.DTOs.Gallery;

namespace Kahrad.Pricing.Products.Application.Validation.Gallery;

public sealed class AddProductImageRequestValidator : AbstractValidator<AddProductImageRequest>
{
    public AddProductImageRequestValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithMessage("Image URL is required.")
            .MaximumLength(1000).WithMessage("Image URL cannot exceed 1000 characters.");

        RuleFor(x => x.AltText)
            .MaximumLength(250).WithMessage("Alt text cannot exceed 250 characters.");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display order cannot be negative.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
    }
}
