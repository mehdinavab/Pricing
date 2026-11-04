using FluentValidation;
using Kahrad.Pricing.BuildingBlocks.Application.Validation.Extensions;
using Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;

namespace Kahrad.Pricing.Guarantees.Application.Validation.Vendors;

public sealed class UpdateVendorRequestValidator : AbstractValidator<UpdateVendorRequest>
{
    public UpdateVendorRequestValidator()
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

        RuleFor(x => x.ContactPerson)
            .MaximumLength(150);

        RuleFor(x => x.PhoneNumber!)
            .ValidIranianMobileNumber()
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(200)
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Address)
            .MaximumLength(1000);

        RuleFor(x => x.Website)
            .MaximumLength(500);

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
