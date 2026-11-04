namespace Kahrad.Pricing.Sms.Application.DTOs;

public class SmsProviderResult
{
    public bool IsSuccess { get; init; }
    public string? ProviderMessageId { get; init; }
    public decimal? Cost { get; init; }
    public string? ErrorMessage { get; init; }

    public static SmsProviderResult Success(string? providerMessageId = null, decimal? cost = null) =>
        new() { IsSuccess = true, ProviderMessageId = providerMessageId, Cost = cost };

    public static SmsProviderResult Failure(string errorMessage) =>
        new() { IsSuccess = false, ErrorMessage = errorMessage };
}
