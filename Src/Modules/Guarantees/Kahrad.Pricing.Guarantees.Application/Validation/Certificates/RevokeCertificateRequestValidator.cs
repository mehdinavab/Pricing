using FluentValidation;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;

namespace Kahrad.Pricing.Guarantees.Application.Validation.Certificates;

public sealed class RevokeCertificateRequestValidator : AbstractValidator<RevokeCertificateRequest>
{
    public RevokeCertificateRequestValidator()
    {
        RuleFor(x => x.Reason)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(500);
    }
}
