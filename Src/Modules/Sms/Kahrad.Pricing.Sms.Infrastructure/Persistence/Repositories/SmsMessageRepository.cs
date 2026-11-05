using Kahrad.Pricing.Sms.Application.Abstractions.Persistence;
using Kahrad.Pricing.Sms.Domain.Entities;
using Kahrad.Pricing.Sms.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Sms.Infrastructure.Persistence.Repositories;

public sealed class SmsMessageRepository(SmsDbContext context) : ISmsMessageRepository
{
    public async Task<SmsMessage?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await context.Messages
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<SmsMessage?> GetByPublicIdAsync(
        Guid publicId,
        CancellationToken cancellationToken = default)
    {
        return await context.Messages
            .FirstOrDefaultAsync(x => x.PublicId == publicId, cancellationToken);
    }

    public async Task<IReadOnlyCollection<SmsMessage>> GetByBatchIdAsync(
        Guid batchId,
        CancellationToken cancellationToken = default)
    {
        return await context.Messages
            .AsNoTracking()
            .Where(x => x.BatchId == batchId)
            .OrderBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SmsMessage>> GetPendingMessagesAsync(
        int batchSize,
        DateTime utcNow,
        CancellationToken cancellationToken = default)
    {
        return await context.Messages
            .Where(x => context.ProviderConfigurations.Any(p => p.Id == x.ProviderConfigurationId && p.IsEnabled))
            .Where(x => x.Status == SmsStatus.Pending && x.SendDateUtc <= utcNow && (!x.NextAttemptAtUtc.HasValue || x.NextAttemptAtUtc <= utcNow) && utcNow <= x.ExpiresAtUtc)
            .OrderBy(x => x.SendDateUtc)
            .ThenBy(x => x.Id)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SmsMessage>> GetExpiredPendingMessagesAsync(
        DateTime utcNow,
        CancellationToken cancellationToken = default)
    {
        return await context.Messages
            .Where(x => x.Status == SmsStatus.Pending && utcNow > x.ExpiresAtUtc)
            .OrderBy(x => x.ExpiresAtUtc)
            .Take(500)
            .ToListAsync(cancellationToken);
    }

    public async Task<long> AddAsync(
        SmsMessage message,
        CancellationToken cancellationToken = default)
    {
        await context.Messages.AddAsync(message, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return message.Id;
    }

    public async Task AddRangeAsync(
        IEnumerable<SmsMessage> messages,
        CancellationToken cancellationToken = default)
    {
        await context.Messages.AddRangeAsync(messages, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        SmsMessage message,
        CancellationToken cancellationToken = default)
    {
        context.Messages.Update(message);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateRangeAsync(
        IEnumerable<SmsMessage> messages,
        CancellationToken cancellationToken = default)
    {
        context.Messages.UpdateRange(messages);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task StartAttemptAsync(SmsMessage message, SmsLog log, CancellationToken cancellationToken = default)
    {
        context.Messages.Update(message);
        context.Logs.Add(log);
        // EF SaveChanges is transactional: the claim and started log either both commit or neither does.
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task CompleteAttemptAsync(SmsMessage message, SmsLog log, CancellationToken cancellationToken = default)
    {
        context.Messages.Update(message);
        context.Logs.Update(log);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SmsLog>> GetLogsAsync(long messageId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (page < 1 || pageSize is < 1 or > 100) throw new ArgumentOutOfRangeException(nameof(pageSize));
        return await context.Logs.AsNoTracking().Where(x => x.SmsMessageId == messageId)
            .OrderBy(x => x.AttemptNumber).ThenBy(x => x.Id)
            .Skip(checked((page - 1) * pageSize)).Take(pageSize).ToListAsync(cancellationToken);
    }

    public IQueryable<SmsMessage> GetQueryable()
    {
        return context.Messages.AsQueryable();
    }
}
