using FluentValidation;
using Kahrad.Pricing.Brands.Application.DTOs.Brands;

namespace Kahrad.Pricing.Brands.Application.Validation.Brands;

public sealed class UpdateBrandRequestValidator : AbstractValidator<UpdateBrandRequest>
{
    public UpdateBrandRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(200);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50)
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage("Code must contain only alphanumeric characters, underscores, or hyphens.");

        RuleFor(x => x.Description)
            .MaximumLength(500);

        RuleFor(x => x.LogoUrl)
            .MaximumLength(1000);

        RuleFor(x => x.Website)
            .MaximumLength(500);

        RuleFor(x => x.SupportPhone)
            .MaximumLength(50);

        RuleFor(x => x.SupportEmail)
            .EmailAddress()
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.SupportEmail));
    }
}
