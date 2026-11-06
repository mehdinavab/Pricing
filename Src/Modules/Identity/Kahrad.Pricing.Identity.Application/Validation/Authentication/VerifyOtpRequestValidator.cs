using FluentValidation;
using Kahrad.Pricing.BuildingBlocks.Application.Validation.Extensions;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;

namespace Kahrad.Pricing.Identity.Application.Validation.Authentication
{
    public sealed class VerifyOtpRequestValidator : AbstractValidator<VerifyOtpRequest>
    {
        public VerifyOtpRequestValidator()
        {
            RuleFor(x => x.RequestId)
                .NotEmpty();

            RuleFor(x => x.PhoneNumber)
             .ValidIranianMobileNumber()
             .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.Code)
                .NotEmpty()
                .Length(4, 10);
        }
    }
}
