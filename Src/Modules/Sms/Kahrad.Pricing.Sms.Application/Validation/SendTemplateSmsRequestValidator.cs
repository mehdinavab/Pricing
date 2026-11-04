using FluentValidation;
using Kahrad.Pricing.BuildingBlocks.Application.Validation.Extensions;
using Kahrad.Pricing.Sms.Application.DTOs;

namespace Kahrad.Pricing.Sms.Application.Validation;

public sealed class SendTemplateSmsRequestValidator : AbstractValidator<SendTemplateSmsRequest>
{
    public SendTemplateSmsRequestValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .ValidIranianMobileNumber();

        RuleFor(x => x.TemplateId)
            .GreaterThan(0).WithMessage("Template ID must be positive.");

        RuleFor(x => x.LineNumber)
            .MaximumLength(30).WithMessage("Line number cannot exceed 30 characters.");
    }
}
