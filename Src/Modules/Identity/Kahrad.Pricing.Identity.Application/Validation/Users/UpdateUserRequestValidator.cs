using FluentValidation;
using Kahrad.Pricing.Identity.Application.DTOs.Users;
using Kahrad.Pricing.BuildingBlocks.Application.Validation.Extensions;


namespace Kahrad.Pricing.Identity.Application.Validation.Users
{
    internal class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .EmailAddress()
                .MaximumLength(256)
                .When(x => !string.IsNullOrWhiteSpace(x.Email));

            RuleFor(x => x.PhoneNumber)
                .ValidIranianMobileNumber()
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));
        }
    }
}
