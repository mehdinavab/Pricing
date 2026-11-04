namespace Kahrad.Pricing.Sms.Application.DTOs;

public class SendPeerToPeerSmsRequest
{
    public List<PeerToPeerSmsItem> Items { get; init; } = [];
    public DateTime? SendDateUtc { get; init; }
    public string? LineNumber { get; init; }
}
