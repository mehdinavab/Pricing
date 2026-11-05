using Kahrad.Pricing.AccountingIntegration.Application.Contracts;

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure.Services;

public sealed class NotConfiguredAccountingProviderClient : IAccountingProviderClient
{
    public Task<AccountingSnapshot<ImportedAccountingItem>> ReadItemsAsync(AccountingConnectionContext connection, CancellationToken cancellationToken)
        => throw new AccountingProviderNotConfiguredException();
    public Task<AccountingSnapshot<ImportedAccountingRepresentative>> ReadRepresentativesAsync(AccountingConnectionContext connection, CancellationToken cancellationToken)
        => throw new AccountingProviderNotConfiguredException();
    public Task<AccountingSnapshot<ImportedAccountingInvoice>> ReadInvoicesAsync(AccountingConnectionContext connection, CancellationToken cancellationToken)
        => throw new AccountingProviderNotConfiguredException();
}
