using System.Text.Json;
using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.Sms.Domain.Entities;

public sealed class SmsProviderConfiguration : BaseEntity
{
    public string Name { get; private set; } = null!;
    public string Provider { get; private set; } = null!;
    public bool IsEnabled { get; private set; }
    public string ApiKeyConfigurationKey { get; private set; } = null!;
    public string? LineNumber { get; private set; }
    public int TimeoutSeconds { get; private set; }
    public string TemplateMappingsJson { get; private set; } = "{}";

    private SmsProviderConfiguration() { }

    public SmsProviderConfiguration(string name, string provider, string apiKeyConfigurationKey,
        string? lineNumber = null, int timeoutSeconds = 30, bool isEnabled = true)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Trim().Length > 100) throw new ArgumentException("Provider name is too long.", nameof(name));
        if (provider is not ("SmsIr" or "Kavenegar")) throw new ArgumentException("Unsupported SMS provider.", nameof(provider));
        Name = name.Trim();
        Provider = provider;
        Configure(apiKeyConfigurationKey, lineNumber, timeoutSeconds, isEnabled);
    }

    public void Configure(string apiKeyConfigurationKey, string? lineNumber, int timeoutSeconds, bool isEnabled)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKeyConfigurationKey);
        if (apiKeyConfigurationKey.Length > 200) throw new ArgumentException("Configuration key is too long.", nameof(apiKeyConfigurationKey));
        if (lineNumber?.Length > 30) throw new ArgumentException("Line number is too long.", nameof(lineNumber));
        if (timeoutSeconds is < 1 or > 300) throw new ArgumentOutOfRangeException(nameof(timeoutSeconds));
        ApiKeyConfigurationKey = apiKeyConfigurationKey.Trim();
        LineNumber = string.IsNullOrWhiteSpace(lineNumber) ? null : lineNumber.Trim();
        TimeoutSeconds = timeoutSeconds;
        IsEnabled = isEnabled;
    }

    public void SetTemplateMappings(IReadOnlyDictionary<int, string> mappings)
    {
        ArgumentNullException.ThrowIfNull(mappings);
        if (mappings.Any(x => x.Key <= 0 || string.IsNullOrWhiteSpace(x.Value) || x.Value.Length > 100))
            throw new ArgumentException("Template IDs and names must be valid.", nameof(mappings));
        var json = JsonSerializer.Serialize(mappings);
        if (json.Length > 4000) throw new ArgumentException("Too many template mappings.", nameof(mappings));
        TemplateMappingsJson = json;
    }

    public Dictionary<int, string> GetTemplateMappings() =>
        JsonSerializer.Deserialize<Dictionary<int, string>>(TemplateMappingsJson) ?? [];
}
