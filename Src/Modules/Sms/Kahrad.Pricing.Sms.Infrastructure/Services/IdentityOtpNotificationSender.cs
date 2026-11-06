using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;

namespace Kahrad.Pricing.Sms.Infrastructure.Services;

public sealed class IdentityOtpNotificationSender(ISmsService smsService)
    : IOtpNotificationSender
{
    public async Task SendAsync(
        string phoneNumber,
        string code,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default)
    {
        await smsService.SendOtpAsync(
            new SendOtpSmsRequest
            {
                PhoneNumber = phoneNumber,
                Code = code,
                ExpiresAtUtc = expiresAtUtc
            },
            cancellationToken: cancellationToken);
    }
}
