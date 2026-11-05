using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.AccountingIntegration.Domain.Entities;

public sealed class AccountingBrandConnection : BaseEntity
{
    private AccountingBrandConnection() { }
    public AccountingBrandConnection(long brandId, string providerCode)
    {
        if (brandId <= 0) throw new ArgumentOutOfRangeException(nameof(brandId));
        ArgumentException.ThrowIfNullOrWhiteSpace(providerCode);
        BrandId = brandId;
        ProviderCode = providerCode.Trim().ToUpperInvariant();
    }

    public long BrandId { get; private set; }
    public string ProviderCode { get; private set; } = null!;
    public string? BaseUrl { get; private set; }
    public string? ApiKeySecretReference { get; private set; }
    public bool IsEnabled { get; private set; }

    public void Configure(string? baseUrl, string? secretReference, bool enabled)
    {
        if (baseUrl is not null && (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("An HTTPS accounting base URL is required.", nameof(baseUrl));
        if (enabled && (string.IsNullOrWhiteSpace(baseUrl) || string.IsNullOrWhiteSpace(secretReference)))
            throw new InvalidOperationException("Accounting connection requires a URL and secret reference before enabling.");
        BaseUrl = baseUrl?.Trim();
        ApiKeySecretReference = secretReference?.Trim();
        IsEnabled = enabled;
    }
}
