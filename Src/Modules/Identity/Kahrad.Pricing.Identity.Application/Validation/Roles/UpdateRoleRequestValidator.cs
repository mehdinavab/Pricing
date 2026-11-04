using FluentValidation;
using Kahrad.Pricing.Identity.Application.DTOs.Roles;

namespace Kahrad.Pricing.Identity.Application.Validation.Roles
{
    public sealed class UpdateRoleRequestValidator : AbstractValidator<UpdateRoleRequest>
    {
        public UpdateRoleRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(500);
        }
    }
}
