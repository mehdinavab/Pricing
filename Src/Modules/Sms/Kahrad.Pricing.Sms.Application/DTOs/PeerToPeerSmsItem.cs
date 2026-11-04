namespace Kahrad.Pricing.Sms.Application.DTOs;

public class PeerToPeerSmsItem
{
    public string PhoneNumber { get; init; } = null!;
    public string Message { get; init; } = null!;
}
