using FluentValidation;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;


namespace Kahrad.Pricing.Identity.Application.Validation.Authentication
{
    public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty();
        }
    }
}
