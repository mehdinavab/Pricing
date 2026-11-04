using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Kahrad.Pricing.Sms.Infrastructure.Background;

public sealed class SmsBackgroundWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<SmsBackgroundWorker> logger) : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(2);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("SMS Background Worker started. Checking for unsent SMS every 2 minutes.");

        // Initial delay after application startup before the first run
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        using var timer = new PeriodicTimer(Interval);

        // Run immediately on start, then every 2 minutes on timer tick
        await ProcessOutboxSafeAsync(stoppingToken);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            await ProcessOutboxSafeAsync(stoppingToken);
        }

        logger.LogInformation("SMS Background Worker is stopping.");
    }

    private async Task ProcessOutboxSafeAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogDebug("SMS Background Worker tick: checking for unsent and expired SMS.");

            using var scope = scopeFactory.CreateScope();
            var outboxProcessor = scope.ServiceProvider.GetRequiredService<ISmsOutboxProcessor>();

            var result = await outboxProcessor.ProcessPendingOutboxAsync(stoppingToken);

            if (result.ProcessedCount > 0)
            {
                logger.LogInformation(
                    "SMS Background Worker completed: Sent={Sent}, Failed={Failed}, Expired={Expired}.",
                    result.SentCount,
                    result.FailedCount,
                    result.ExpiredCount);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // Graceful shutdown
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred during SMS background outbox processing.");
        }
    }
}
