namespace Kahrad.Pricing.Identity.Application.DTOs.Authentication;

public sealed record OtpRequestResponse(
    Guid RequestId,
    DateTime ExpiresAtUtc,
    int RetryAfterSeconds);
