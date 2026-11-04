using FluentValidation;
using Kahrad.Pricing.BuildingBlocks.Application.Validation.Extensions;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;

namespace Kahrad.Pricing.Guarantees.Application.Validation.Certificates;

public sealed class IssueCertificateRequestValidator : AbstractValidator<IssueCertificateRequest>
{
    public IssueCertificateRequestValidator()
    {
        RuleFor(x => x.GuaranteeId)
            .GreaterThan(0)
            .WithMessage("GuaranteeId must be a valid positive identifier.");

        RuleFor(x => x.ProductId)
            .GreaterThan(0)
            .WithMessage("ProductId must be a valid positive identifier.");

        RuleFor(x => x.PurchaseDate)
            .NotEmpty()
            .WithMessage("PurchaseDate is required.");

        RuleFor(x => x.CertificateNumber)
            .MaximumLength(100)
            .When(x => !string.IsNullOrWhiteSpace(x.CertificateNumber));

        RuleFor(x => x.ProductSerialNumber)
            .MaximumLength(100);

        RuleFor(x => x.CustomerName)
            .MaximumLength(200);

        RuleFor(x => x.CustomerPhoneNumber!)
            .ValidIranianMobileNumber()
            .When(x => !string.IsNullOrWhiteSpace(x.CustomerPhoneNumber));

        RuleFor(x => x.CustomerNationalCode!)
            .ValidIranianNationalCode()
            .When(x => !string.IsNullOrWhiteSpace(x.CustomerNationalCode));

        RuleFor(x => x.Description)
            .MaximumLength(500);
    }
}
