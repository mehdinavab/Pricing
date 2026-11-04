using FluentValidation;
using Kahrad.Pricing.BuildingBlocks.Application.Validation.Extensions;
using Kahrad.Pricing.Sms.Application.DTOs;

namespace Kahrad.Pricing.Sms.Application.Validation;

public sealed class SendBulkSmsRequestValidator : AbstractValidator<SendBulkSmsRequest>
{
    public SendBulkSmsRequestValidator()
    {
        RuleFor(x => x.PhoneNumbers)
            .NotEmpty().WithMessage("At least one phone number is required.")
            .Must(list => list.Count <= 1000).WithMessage("Bulk SMS cannot exceed 1000 phone numbers per request.");

        RuleForEach(x => x.PhoneNumbers)
            .ValidIranianMobileNumber();

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message text is required.")
            .MaximumLength(1000).WithMessage("Message cannot exceed 1000 characters.");

        RuleFor(x => x.LineNumber)
            .MaximumLength(30).WithMessage("Line number cannot exceed 30 characters.");
    }
}
