using FluentValidation;
using Kahrad.Pricing.Identity.Application.DTOs.Users;

namespace Kahrad.Pricing.Identity.Application.Validation.Users
{
    public sealed class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(12)
                .MaximumLength(128);
        }
    }
}
