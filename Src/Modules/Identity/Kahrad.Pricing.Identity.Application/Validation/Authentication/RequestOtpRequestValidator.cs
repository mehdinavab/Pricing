using FluentValidation;
using Kahrad.Pricing.BuildingBlocks.Application.Validation.Extensions;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;

namespace Kahrad.Pricing.Identity.Application.Validation.Authentication
{
    public sealed class RequestOtpRequestValidator : AbstractValidator<RequestOtpRequest>
    {
        public RequestOtpRequestValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .ValidIranianMobileNumber()
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        }
    }
}
