using Kahrad.Pricing.AccountingIntegration.Domain.Entities;

namespace Kahrad.Pricing.AccountingIntegration.Application.Contracts;

public interface IAccountingMirrorStore
{
    Task<AccountingConnectionContext?> GetConnectionAsync(long brandId, CancellationToken cancellationToken);
    Task<long> BeginRunAsync(long brandId, AccountingSyncType type, CancellationToken cancellationToken);
    Task CompleteItemsAsync(long runId, long brandId, AccountingSnapshot<ImportedAccountingItem> snapshot, CancellationToken cancellationToken);
    Task CompleteRepresentativesAsync(long runId, long brandId, AccountingSnapshot<ImportedAccountingRepresentative> snapshot, CancellationToken cancellationToken);
    Task CompleteInvoicesAsync(long runId, long brandId, AccountingSnapshot<ImportedAccountingInvoice> snapshot, CancellationToken cancellationToken);
    Task FailRunAsync(long runId, string errorCategory, CancellationToken cancellationToken);
}
