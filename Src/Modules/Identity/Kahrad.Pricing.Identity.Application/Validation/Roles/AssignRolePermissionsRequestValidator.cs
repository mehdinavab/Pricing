using FluentValidation;
using Kahrad.Pricing.Identity.Application.DTOs.Roles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kahrad.Pricing.Identity.Application.Validation.Roles
{
    public sealed class AssignRolePermissionsRequestValidator : AbstractValidator<AssignRolePermissionsRequest>
    {
        public AssignRolePermissionsRequestValidator()
        {
            RuleFor(x => x.PermissionIds)
                .NotNull()
                .NotEmpty();

            RuleForEach(x => x.PermissionIds)
                .GreaterThan(0);
        }
    }
}
