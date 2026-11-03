using FluentValidation;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kahrad.Pricing.Identity.Application.Validation.Authentication
{
    public sealed class PasswordLoginRequestValidator : AbstractValidator<PasswordLoginRequest>
    {
        public PasswordLoginRequestValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
