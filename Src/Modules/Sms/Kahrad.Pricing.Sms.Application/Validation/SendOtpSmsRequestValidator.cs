using FluentValidation;
using Kahrad.Pricing.BuildingBlocks.Application.Validation.Extensions;
using Kahrad.Pricing.Sms.Application.DTOs;

namespace Kahrad.Pricing.Sms.Application.Validation;

public sealed class SendOtpSmsRequestValidator : AbstractValidator<SendOtpSmsRequest>
{
    public SendOtpSmsRequestValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .ValidIranianMobileNumber();

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("OTP code is required.")
            .MaximumLength(20).WithMessage("OTP code cannot exceed 20 characters.");

        RuleFor(x => x.TemplateId)
            .GreaterThan(0).When(x => x.TemplateId.HasValue).WithMessage("Template ID must be positive.");

        RuleFor(x => x.LineNumber)
            .MaximumLength(30).WithMessage("Line number cannot exceed 30 characters.");
    }
}
