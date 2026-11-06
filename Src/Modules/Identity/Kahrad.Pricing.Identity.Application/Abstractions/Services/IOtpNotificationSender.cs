namespace Kahrad.Pricing.Identity.Application.Abstractions.Services;

public interface IOtpNotificationSender
{
    Task SendAsync(
        string phoneNumber,
        string code,
        DateTime expiresAtUtc,
        CancellationToken cancellationToken = default);
}
