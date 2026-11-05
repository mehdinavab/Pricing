using System.Text.RegularExpressions;
using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.AccountingIntegration.Domain.Entities;
using Kahrad.Pricing.Brands.Application.Abstractions.Services;

namespace Kahrad.Pricing.AccountingIntegration.Application.Services;

public sealed class AccountingConnectionService(IAccountingConnectionStore store, IBrandService brands)
{
    private static readonly Regex SecretReference = new("^env:[A-Za-z_][A-Za-z0-9_]{0,127}$", RegexOptions.Compiled);

    public Task<IReadOnlyList<AccountingConnectionView>> ListAsync(CancellationToken ct) => store.ListAsync(ct);

    public async Task SaveAsync(long brandId, string? baseUrl, string? secretReference, CancellationToken ct)
    {
        if (brandId <= 0 || await brands.GetByIdAsync(brandId, ct) is null)
            throw new AccountingOperationException("Brand was not found for accounting connection.");
        if (secretReference is not null && !SecretReference.IsMatch(secretReference))
            throw new AccountingOperationException("Use an environment secret reference, not an API key.");
        if (baseUrl is not null && (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps))
            throw new AccountingOperationException("An HTTPS accounting base URL is required.");
        var connection = await store.GetByBrandAsync(brandId, ct)
            ?? new AccountingBrandConnection(brandId, "MAIN");
        connection.Configure(baseUrl, secretReference ?? connection.ApiKeySecretReference, false);
        await store.SaveAsync(connection, ct);
    }
}
