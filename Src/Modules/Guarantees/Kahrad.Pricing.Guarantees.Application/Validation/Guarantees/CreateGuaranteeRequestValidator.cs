using FluentValidation;
using Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;

namespace Kahrad.Pricing.Guarantees.Application.Validation.Guarantees;

public sealed class CreateGuaranteeRequestValidator : AbstractValidator<CreateGuaranteeRequest>
{
    public CreateGuaranteeRequestValidator()
    {
        RuleFor(x => x.VendorId)
            .GreaterThan(0)
            .WithMessage("VendorId must be a valid positive identifier.");

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

        RuleFor(x => x.DurationMonths)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x.DurationDays)
            .GreaterThanOrEqualTo(0);

        RuleFor(x => x)
            .Must(x => x.DurationMonths > 0 || x.DurationDays > 0)
            .WithMessage("Guarantee must have a duration greater than 0 days.");

        RuleFor(x => x.Description)
            .MaximumLength(1000);

        RuleFor(x => x.TermsAndConditions)
            .MaximumLength(4000);
    }
}
