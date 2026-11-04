using Kahrad.Pricing.Sms.Application.DTOs;

namespace Kahrad.Pricing.Sms.Application.Abstractions.Services;

public interface ISmsOutboxProcessor
{
    Task<SmsOutboxProcessResult> ProcessPendingOutboxAsync(
        CancellationToken cancellationToken = default);
}
