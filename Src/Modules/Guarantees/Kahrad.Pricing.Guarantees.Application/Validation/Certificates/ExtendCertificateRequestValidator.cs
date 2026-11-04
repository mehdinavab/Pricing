using FluentValidation;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;

namespace Kahrad.Pricing.Guarantees.Application.Validation.Certificates;

public sealed class ExtendCertificateRequestValidator : AbstractValidator<ExtendCertificateRequest>
{
    public ExtendCertificateRequestValidator()
    {
        RuleFor(x => x.AdditionalMonths)
            .GreaterThan(0)
            .LessThanOrEqualTo(120)
            .WithMessage("Additional months must be between 1 and 120.");
    }
}
