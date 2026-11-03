using FluentValidation;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Kahrad.Pricing.BuildingBlocks.Application.Validation.Extensions;

namespace Kahrad.Pricing.Identity.Application.Validation.Authentication
{

    public sealed class OtpLoginRequestValidator : AbstractValidator<OtpLoginRequest>
    {
        public OtpLoginRequestValidator()
        {
            RuleFor(x => x.PhoneNumber)
       .ValidIranianMobileNumber()
       .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.Code)
                .NotEmpty()
                .Length(4, 10);
        }
    }
}
