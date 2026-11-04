using FluentValidation;
using Kahrad.Pricing.BuildingBlocks.Application.Validation.Extensions;
using Kahrad.Pricing.Sms.Application.DTOs;

namespace Kahrad.Pricing.Sms.Application.Validation;

public sealed class SendPeerToPeerSmsRequestValidator : AbstractValidator<SendPeerToPeerSmsRequest>
{
    public SendPeerToPeerSmsRequestValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one peer-to-peer item is required.")
            .Must(list => list.Count <= 500).WithMessage("Peer-to-peer SMS cannot exceed 500 items per request.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(i => i.PhoneNumber)
                .ValidIranianMobileNumber();

            item.RuleFor(i => i.Message)
                .NotEmpty().WithMessage("Message text is required.")
                .MaximumLength(1000).WithMessage("Message cannot exceed 1000 characters.");
        });

        RuleFor(x => x.LineNumber)
            .MaximumLength(30).WithMessage("Line number cannot exceed 30 characters.");
    }
}
