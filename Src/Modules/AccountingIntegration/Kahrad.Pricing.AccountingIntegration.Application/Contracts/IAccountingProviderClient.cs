using Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

namespace Kahrad.Pricing.AccountingIntegration.Application.Contracts;

/// <summary>Normalized application boundary; the provider's HTTP DTOs and paging are not known yet.</summary>
public interface IAccountingProviderClient
{
    Task<AccountingSnapshot<ImportedAccountingItem>> ReadItemsAsync(AccountingConnectionContext connection, CancellationToken cancellationToken);
    Task<AccountingSnapshot<ImportedAccountingRepresentative>> ReadRepresentativesAsync(AccountingConnectionContext connection, CancellationToken cancellationToken);
    Task<AccountingSnapshot<ImportedAccountingInvoice>> ReadInvoicesAsync(AccountingConnectionContext connection, CancellationToken cancellationToken);
}

public sealed record AccountingConnectionContext(long BrandId, string ProviderCode, string BaseUrl, string ApiKeySecretReference);
public sealed record AccountingSnapshot<T>(IReadOnlyList<T> Records, bool IsComplete);
public sealed record ImportedAccountingItem(string ExternalItemId, string Name, string? Code, string? UnitCode,
    decimal? Price, string? CurrencyCode);
public sealed record ImportedAccountingRepresentative(string ExternalRepresentativeId, string Name, string? Code);
public sealed record ImportedAccountingInvoice(string ExternalInvoiceId, string? ExternalRepresentativeId,
    string? InvoiceNumber, string? ExternalStatus, DateTime? IssuedAtUtc);

public sealed class AccountingProviderNotConfiguredException()
    : KahradPricingException(
        "Accounting provider contract is not configured.",
        true,
        ErrorCodes.ServiceUnavailable);
