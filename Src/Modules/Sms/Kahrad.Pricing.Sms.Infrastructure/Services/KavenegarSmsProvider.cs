using System.Text.Json;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Application.DTOs;
using Kahrad.Pricing.Sms.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Kahrad.Pricing.Sms.Infrastructure.Services;

public sealed class KavenegarSmsProvider(KavenegarTransport transport, IOptions<KavenegarOptions> options) : ISmsProvider
{
    public Task<SmsProviderResult> SendBulkAsync(IEnumerable<string> phoneNumbers, string message,
        string? lineNumber = null, CancellationToken cancellationToken = default) =>
        SendAsync("sms/send", new()
        {
            ["receptor"] = string.Join(",", phoneNumbers), ["message"] = message,
            ["sender"] = lineNumber ?? options.Value.LineNumber ?? ""
        }, cancellationToken);

    public Task<SmsProviderResult> SendPeerToPeerAsync(IEnumerable<PeerToPeerSmsItem> items,
        string? lineNumber = null, CancellationToken cancellationToken = default)
    {
        var recipients = items.ToArray();
        return SendAsync("sms/sendarray", new()
        {
            ["receptor"] = JsonSerializer.Serialize(recipients.Select(x => x.PhoneNumber)),
            ["message"] = JsonSerializer.Serialize(recipients.Select(x => x.Message)),
            ["sender"] = JsonSerializer.Serialize(recipients.Select(_ => lineNumber ?? options.Value.LineNumber ?? ""))
        }, cancellationToken);
    }

    public Task<SmsProviderResult> SendTemplateAsync(string phoneNumber, int templateId,
        IDictionary<string, string> parameters, string? lineNumber = null, CancellationToken cancellationToken = default)
    {
        if (!options.Value.Templates.TryGetValue(templateId, out var template) || string.IsNullOrWhiteSpace(template))
            return Task.FromResult(SmsProviderResult.Failure("Kavenegar template mapping is not configured."));
        var fields = new Dictionary<string, string> { ["receptor"] = phoneNumber, ["template"] = template };
        foreach (var parameter in parameters)
        {
            var key = parameter.Key.Equals("Code", StringComparison.OrdinalIgnoreCase) ? "token" : parameter.Key.ToLowerInvariant();
            if (key is not ("token" or "token2" or "token3" or "token10" or "token20"))
                return Task.FromResult(SmsProviderResult.Failure("Unsupported Kavenegar template parameter."));
            if (!fields.TryAdd(key, parameter.Value))
                return Task.FromResult(SmsProviderResult.Failure("Duplicate Kavenegar template token."));
        }
        return SendAsync("verify/lookup", fields, cancellationToken);
    }

    private async Task<SmsProviderResult> SendAsync(string method, Dictionary<string, string> fields, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(options.Value.ApiKey))
            return SmsProviderResult.Failure("Kavenegar API key is not configured.");
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromSeconds(options.Value.TimeoutSeconds));
        try
        {
            using var content = new FormUrlEncodedContent(fields);
            using var response = await transport.Client.PostAsync(
                $"https://api.kavenegar.com/v1/{Uri.EscapeDataString(options.Value.ApiKey)}/{method}.json", content, timeout.Token);
            using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync(timeout.Token));
            var status = json.RootElement.GetProperty("return").GetProperty("status").GetInt32();
            if (status != 200)
                // 409 explicitly means the server cannot respond and requests a later attempt.
                return SmsProviderResult.Failure($"Kavenegar rejected request (status {status}).", status == 409);
            if (!response.IsSuccessStatusCode)
                return SmsProviderResult.Failure("Inconsistent Kavenegar response; investigate delivery before resending.", outcomeUnknown: true);
            var entries = json.RootElement.GetProperty("entries");
            if (entries.ValueKind != JsonValueKind.Array || entries.GetArrayLength() == 0)
                return SmsProviderResult.Failure("Kavenegar returned no message receipt; delivery requires investigation.", outcomeUnknown: true);
            // The outbox submits one recipient at a time so each receipt belongs to one message.
            return SmsProviderResult.Success(entries[0].GetProperty("messageid").ToString(),
                entries[0].TryGetProperty("cost", out var cost) ? cost.GetDecimal() : null);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (Exception ex) when (ex is HttpRequestException or OperationCanceledException or JsonException or InvalidOperationException or KeyNotFoundException or FormatException)
        {
            // A timeout/connection failure can happen after acceptance. Never blindly resend.
            return SmsProviderResult.Failure("Kavenegar delivery outcome is unknown; investigate before resending.", outcomeUnknown: true);
        }
    }
}

public sealed class KavenegarTransport(HttpClient client) : IDisposable
{
    public HttpClient Client { get; } = client;
    public void Dispose() => Client.Dispose();
}
