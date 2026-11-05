using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.AccountingIntegration.Domain.Entities;

public enum AccountingSyncType { Items, Representatives, Invoices }
public enum AccountingSyncStatus { Running, Succeeded, Failed }

public sealed class AccountingSyncRun : BaseEntity
{
    private AccountingSyncRun() { }
    public AccountingSyncRun(long brandId, AccountingSyncType type, DateTime startedAtUtc)
    {
        if (brandId <= 0) throw new ArgumentOutOfRangeException(nameof(brandId));
        BrandId = brandId; Type = type; StartedAtUtc = startedAtUtc;
    }

    public long BrandId { get; private set; }
    public AccountingSyncType Type { get; private set; }
    public AccountingSyncStatus Status { get; private set; } = AccountingSyncStatus.Running;
    public DateTime StartedAtUtc { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    public int RowsFetched { get; private set; }
    public int RowsChanged { get; private set; }
    public bool FullScanCompleted { get; private set; }
    public string? ErrorCategory { get; private set; }

    public void Complete(int fetched, int changed, bool fullScanCompleted, DateTime completedAtUtc)
    {
        if (!fullScanCompleted || fetched < 0 || changed < 0) throw new InvalidOperationException("A partial accounting scan cannot be marked successful.");
        RowsFetched = fetched; RowsChanged = changed; FullScanCompleted = true;
        Status = AccountingSyncStatus.Succeeded; CompletedAtUtc = completedAtUtc;
    }

    public void Fail(string errorCategory, DateTime completedAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(errorCategory);
        ErrorCategory = errorCategory.Length <= 100 ? errorCategory : errorCategory[..100];
        Status = AccountingSyncStatus.Failed; CompletedAtUtc = completedAtUtc;
    }
}
