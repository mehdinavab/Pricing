using Kahrad.Pricing.Sms.Domain.Enums;
using Sieve.Models;

namespace Kahrad.Pricing.Sms.Application.DTOs;

public class SmsQueryDto : SieveModel
{
    public SmsStatus? Status { get; init; }
    public SmsType? Type { get; init; }
    public string? PhoneNumber { get; init; }
    public Guid? BatchId { get; init; }
}
