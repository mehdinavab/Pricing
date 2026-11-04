using FluentValidation;
using Kahrad.Pricing.Identity.Application.DTOs.Users;

namespace Kahrad.Pricing.Identity.Application.Validation.Users
{
    public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty();

            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(12)
                .MaximumLength(128)
                .NotEqual(x => x.CurrentPassword)
                .WithMessage("The new password must be different from the current password.");
        }
    }
}
