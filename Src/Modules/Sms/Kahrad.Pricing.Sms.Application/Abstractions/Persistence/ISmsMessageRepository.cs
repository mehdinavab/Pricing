using Kahrad.Pricing.Sms.Domain.Entities;

namespace Kahrad.Pricing.Sms.Application.Abstractions.Persistence;

public interface ISmsMessageRepository
{
    Task<SmsMessage?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default);

    Task<SmsMessage?> GetByPublicIdAsync(
        Guid publicId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SmsMessage>> GetByBatchIdAsync(
        Guid batchId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SmsMessage>> GetPendingMessagesAsync(
        int batchSize,
        DateTime utcNow,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<SmsMessage>> GetExpiredPendingMessagesAsync(
        DateTime utcNow,
        CancellationToken cancellationToken = default);

    Task<long> AddAsync(
        SmsMessage message,
        CancellationToken cancellationToken = default);

    Task AddRangeAsync(
        IEnumerable<SmsMessage> messages,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        SmsMessage message,
        CancellationToken cancellationToken = default);

    Task UpdateRangeAsync(
        IEnumerable<SmsMessage> messages,
        CancellationToken cancellationToken = default);

    IQueryable<SmsMessage> GetQueryable();
}
