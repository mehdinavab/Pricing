using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.AccountingIntegration.Domain.Entities;

namespace Kahrad.Pricing.AccountingIntegration.Application.Services;

public sealed class AccountingSyncService(IAccountingProviderClient provider, IAccountingMirrorStore store)
{
    public async Task<long> SyncAsync(long brandId, AccountingSyncType type, CancellationToken cancellationToken)
    {
        if (brandId <= 0) throw new AccountingOperationException("Brand ID must be positive.");
        var connection = await store.GetConnectionAsync(brandId, cancellationToken)
            ?? throw new AccountingProviderNotConfiguredException();
        var runId = await store.BeginRunAsync(brandId, type, cancellationToken);
        try
        {
            switch (type)
            {
                case AccountingSyncType.Items:
                    await store.CompleteItemsAsync(runId, brandId,
                        await provider.ReadItemsAsync(connection, cancellationToken), cancellationToken);
                    break;
                case AccountingSyncType.Representatives:
                    await store.CompleteRepresentativesAsync(runId, brandId,
                        await provider.ReadRepresentativesAsync(connection, cancellationToken), cancellationToken);
                    break;
                case AccountingSyncType.Invoices:
                    await store.CompleteInvoicesAsync(runId, brandId,
                        await provider.ReadInvoicesAsync(connection, cancellationToken), cancellationToken);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(type));
            }
            return runId;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            await store.FailRunAsync(runId, "Cancelled", CancellationToken.None);
            throw;
        }
        catch (Exception exception)
        {
            await store.FailRunAsync(runId, exception is AccountingProviderNotConfiguredException
                ? "NotConfigured" : "SyncFailed", CancellationToken.None);
            throw;
        }
    }
}
