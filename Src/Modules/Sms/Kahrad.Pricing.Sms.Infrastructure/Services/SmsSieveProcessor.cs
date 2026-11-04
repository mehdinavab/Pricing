using Kahrad.Pricing.Sms.Domain.Entities;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace Kahrad.Pricing.Sms.Infrastructure.Services;

public sealed class SmsSieveProcessor(IOptions<SieveOptions> options)
    : SieveProcessor(options)
{
    protected override SievePropertyMapper MapProperties(
        SievePropertyMapper mapper)
    {
        mapper = base.MapProperties(mapper);

        mapper.Property<SmsMessage>(m => m.Id).CanSort();
        mapper.Property<SmsMessage>(m => m.BatchId).CanFilter().CanSort();
        mapper.Property<SmsMessage>(m => m.PhoneNumber).CanFilter().CanSort();
        mapper.Property<SmsMessage>(m => m.Type).CanFilter().CanSort();
        mapper.Property<SmsMessage>(m => m.Status).CanFilter().CanSort();
        mapper.Property<SmsMessage>(m => m.TemplateId).CanFilter().CanSort();
        mapper.Property<SmsMessage>(m => m.LineNumber).CanFilter().CanSort();
        mapper.Property<SmsMessage>(m => m.SendDateUtc).CanFilter().CanSort();
        mapper.Property<SmsMessage>(m => m.ExpiresAtUtc).CanFilter().CanSort();
        mapper.Property<SmsMessage>(m => m.SentAtUtc).CanFilter().CanSort();
        mapper.Property<SmsMessage>(m => m.RetryCount).CanSort();
        mapper.Property<SmsMessage>(m => m.CreatedAtUtc).CanSort();

        return mapper;
    }
}
