using Kahrad.Pricing.AccountingIntegration.Domain.Entities;

namespace Kahrad.Pricing.AccountingIntegration.Application.Contracts;

public sealed record AccountingSyncRunView(long Id, long BrandId, AccountingSyncType Type,
    AccountingSyncStatus Status, DateTime StartedAtUtc, DateTime? CompletedAtUtc,
    int RowsFetched, int RowsChanged, bool FullScanCompleted, string? ErrorCategory);

public interface IAccountingSyncRunReader
{
    Task<IReadOnlyList<AccountingSyncRunView>> SearchAsync(long brandId, AccountingSyncType? type,
        int page, int pageSize, CancellationToken cancellationToken);
}
