using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.AccountingIntegration.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;

public sealed class AccountingMirrorStore(AccountingDbContext db) : IAccountingMirrorStore
{
    public async Task<AccountingConnectionContext?> GetConnectionAsync(long brandId, CancellationToken cancellationToken)
    {
        var connection = await db.Connections.AsNoTracking().SingleOrDefaultAsync(
            x => x.BrandId == brandId && x.IsEnabled && !x.IsDeleted, cancellationToken);
        return connection is { BaseUrl: not null, ApiKeySecretReference: not null }
            ? new(brandId, connection.ProviderCode, connection.BaseUrl, connection.ApiKeySecretReference)
            : null;
    }

    public async Task<long> BeginRunAsync(long brandId, AccountingSyncType type, CancellationToken cancellationToken)
    {
        var run = new AccountingSyncRun(brandId, type, DateTime.UtcNow);
        db.SyncRuns.Add(run);
        await db.SaveChangesAsync(cancellationToken);
        return run.Id;
    }

    public async Task CompleteItemsAsync(long runId, long brandId, AccountingSnapshot<ImportedAccountingItem> snapshot, CancellationToken cancellationToken)
    {
        RequireComplete(snapshot.IsComplete);
        var records = snapshot.Records;
        ValidateUnique(records.Select(x => x.ExternalItemId));
        if (records.Any(x => string.IsNullOrWhiteSpace(x.Name) || string.IsNullOrWhiteSpace(x.ExternalItemId) ||
            x.Price < 0 || (x.Price.HasValue && string.IsNullOrWhiteSpace(x.CurrencyCode))))
            throw new InvalidOperationException("Accounting item snapshot contains invalid data.");

        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var run = await RequireRunAsync(runId, brandId, AccountingSyncType.Items, cancellationToken);
        var existing = await db.Items.Where(x => x.BrandId == brandId)
            .ToDictionaryAsync(x => x.ExternalItemId, cancellationToken);
        var changed = 0;
        var seenAt = DateTime.UtcNow;
        foreach (var record in records)
        {
            if (existing.TryGetValue(record.ExternalItemId, out var item))
                item.Update(record.Name, record.Code, record.UnitCode, seenAt);
            else
            {
                item = new AccountingItem(brandId, record.ExternalItemId, record.Name, record.Code, record.UnitCode, seenAt);
                db.Items.Add(item); existing.Add(record.ExternalItemId, item); changed++;
            }
        }
        await db.SaveChangesAsync(cancellationToken);

        var latest = await db.ItemPrices.AsNoTracking().Where(x => x.BrandId == brandId)
            .GroupBy(x => x.AccountingItemId)
            .Select(group => group.OrderByDescending(x => x.Id).First())
            .ToDictionaryAsync(x => x.AccountingItemId, cancellationToken);
        foreach (var record in records.Where(x => x.Price.HasValue))
        {
            var item = existing[record.ExternalItemId];
            if (latest.TryGetValue(item.Id, out var price) && price.Amount == record.Price &&
                price.CurrencyCode == record.CurrencyCode!.ToUpperInvariant()) continue;
            db.ItemPrices.Add(new AccountingItemPrice(brandId, item.Id, record.Price!.Value,
                record.CurrencyCode!, runId, seenAt));
            changed++;
        }
        var presentIds = records.Select(x => x.ExternalItemId).ToHashSet(StringComparer.Ordinal);
        foreach (var item in existing.Values.Where(x => !presentIds.Contains(x.ExternalItemId) && x.IsPresentInLatestSync))
        {
            item.MarkAbsent(); changed++;
        }
        run.Complete(records.Count, changed, true, seenAt);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task CompleteRepresentativesAsync(long runId, long brandId,
        AccountingSnapshot<ImportedAccountingRepresentative> snapshot, CancellationToken cancellationToken)
    {
        RequireComplete(snapshot.IsComplete);
        ValidateUnique(snapshot.Records.Select(x => x.ExternalRepresentativeId));
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var run = await RequireRunAsync(runId, brandId, AccountingSyncType.Representatives, cancellationToken);
        var existing = await db.Representatives.Where(x => x.BrandId == brandId)
            .ToDictionaryAsync(x => x.ExternalRepresentativeId, cancellationToken);
        var now = DateTime.UtcNow; var changed = 0;
        foreach (var record in snapshot.Records)
        {
            if (existing.TryGetValue(record.ExternalRepresentativeId, out var representative))
                representative.Update(record.Name, record.Code, now);
            else
            {
                representative = new AccountingRepresentative(brandId, record.ExternalRepresentativeId, record.Name, record.Code, now);
                db.Representatives.Add(representative); changed++;
            }
        }
        var seen = snapshot.Records.Select(x => x.ExternalRepresentativeId).ToHashSet(StringComparer.Ordinal);
        foreach (var representative in existing.Values.Where(x => !seen.Contains(x.ExternalRepresentativeId) && x.IsPresentInLatestSync))
        {
            representative.MarkAbsent(); changed++;
        }
        run.Complete(snapshot.Records.Count, changed, true, now);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task CompleteInvoicesAsync(long runId, long brandId,
        AccountingSnapshot<ImportedAccountingInvoice> snapshot, CancellationToken cancellationToken)
    {
        RequireComplete(snapshot.IsComplete);
        ValidateUnique(snapshot.Records.Select(x => x.ExternalInvoiceId));
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var run = await RequireRunAsync(runId, brandId, AccountingSyncType.Invoices, cancellationToken);
        var existing = await db.Invoices.Where(x => x.BrandId == brandId)
            .ToDictionaryAsync(x => x.ExternalInvoiceId, cancellationToken);
        var now = DateTime.UtcNow; var changed = 0;
        foreach (var record in snapshot.Records)
        {
            if (!existing.TryGetValue(record.ExternalInvoiceId, out var invoice))
            {
                invoice = new AccountingInvoice(brandId, record.ExternalInvoiceId, now);
                db.Invoices.Add(invoice); changed++;
            }
            invoice.Update(record.ExternalRepresentativeId, record.InvoiceNumber, record.ExternalStatus, record.IssuedAtUtc, now);
        }
        run.Complete(snapshot.Records.Count, changed, true, now);
        await db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task FailRunAsync(long runId, string errorCategory, CancellationToken cancellationToken)
    {
        var run = await db.SyncRuns.SingleAsync(x => x.Id == runId, cancellationToken);
        if (run.Status != AccountingSyncStatus.Running) return;
        run.Fail(errorCategory, DateTime.UtcNow);
        await db.SaveChangesAsync(cancellationToken);
    }

    private async Task<AccountingSyncRun> RequireRunAsync(long id, long brandId, AccountingSyncType type, CancellationToken ct)
    {
        var run = await db.SyncRuns.SingleAsync(x => x.Id == id && x.BrandId == brandId && x.Type == type, ct);
        if (run.Status != AccountingSyncStatus.Running) throw new InvalidOperationException("Accounting sync run is not active.");
        return run;
    }

    private static void RequireComplete(bool complete)
    {
        if (!complete) throw new InvalidOperationException("Incomplete accounting snapshots cannot be applied.");
    }

    private static void ValidateUnique(IEnumerable<string> ids)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var id in ids)
            if (string.IsNullOrWhiteSpace(id) || !seen.Add(id))
                throw new InvalidOperationException("Accounting snapshot contains a missing or duplicate external ID.");
    }
}
