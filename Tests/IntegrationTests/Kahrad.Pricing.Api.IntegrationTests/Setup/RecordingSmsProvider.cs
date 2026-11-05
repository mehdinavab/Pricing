using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Kahrad.Pricing.Sms.Domain.Entities;
namespace Kahrad.Pricing.Api.IntegrationTests.Setup;

public sealed class RecordingSmsProvider : ISmsProvider
{
    public int Calls { get; private set; }
    public SmsProviderResult Result { get; set; } = SmsProviderResult.Success("test-receipt",100m);
    public void Reset() { Calls = 0; Result = SmsProviderResult.Success("test-receipt",100m); }
    private Task<SmsProviderResult> Record(CancellationToken ct) { ct.ThrowIfCancellationRequested(); Calls++; return Task.FromResult(Result); }
    public Task<SmsProviderResult> SendBulkAsync(IEnumerable<string> numbers,string message,string? lineNumber=null,CancellationToken cancellationToken=default) => Record(cancellationToken);
    public Task<SmsProviderResult> SendPeerToPeerAsync(IEnumerable<PeerToPeerSmsItem> items,string? lineNumber=null,CancellationToken cancellationToken=default) => Record(cancellationToken);
    public Task<SmsProviderResult> SendTemplateAsync(string number,int templateId,IDictionary<string,string> parameters,string? lineNumber=null,CancellationToken cancellationToken=default) => Record(cancellationToken);
}
internal sealed class RecordingSmsProviderResolver(RecordingSmsProvider provider) : ISmsProviderResolver
{
    public ISmsProvider Resolve(SmsProviderConfiguration configuration) => provider;
}
