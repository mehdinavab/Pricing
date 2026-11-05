using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.Sms.Domain.Entities;

public sealed class SmsModuleConfiguration : BaseEntity
{
    public string ModuleKey { get; private set; } = "Sms";
    public long ActiveProviderConfigurationId { get; private set; }
    public bool IsEnabled { get; private set; }
    public int BatchSize { get; private set; }
    public bool RetryEnabled { get; private set; }
    public int MaxAttempts { get; private set; }
    public int InitialRetryDelaySeconds { get; private set; }
    public int MaxRetryDelaySeconds { get; private set; }

    private SmsModuleConfiguration() { }

    public SmsModuleConfiguration(long activeProviderConfigurationId, bool isEnabled = true, int batchSize = 100,
        bool retryEnabled = true, int maxAttempts = 3, int initialRetryDelaySeconds = 120, int maxRetryDelaySeconds = 3600)
    {
        Configure(activeProviderConfigurationId, isEnabled, batchSize, retryEnabled, maxAttempts,
            initialRetryDelaySeconds, maxRetryDelaySeconds);
    }

    public void Configure(long activeProviderConfigurationId, bool isEnabled, int batchSize,
        bool retryEnabled, int maxAttempts, int initialRetryDelaySeconds, int maxRetryDelaySeconds)
    {
        if (activeProviderConfigurationId <= 0) throw new ArgumentOutOfRangeException(nameof(activeProviderConfigurationId));
        if (batchSize is < 1 or > 200) throw new ArgumentOutOfRangeException(nameof(batchSize));
        if (maxAttempts is < 1 or > 20) throw new ArgumentOutOfRangeException(nameof(maxAttempts));
        if (initialRetryDelaySeconds <= 0 || maxRetryDelaySeconds < initialRetryDelaySeconds || maxRetryDelaySeconds > 86400)
            throw new ArgumentOutOfRangeException(nameof(initialRetryDelaySeconds));
        ActiveProviderConfigurationId = activeProviderConfigurationId;
        IsEnabled = isEnabled;
        BatchSize = batchSize;
        RetryEnabled = retryEnabled;
        MaxAttempts = maxAttempts;
        InitialRetryDelaySeconds = initialRetryDelaySeconds;
        MaxRetryDelaySeconds = maxRetryDelaySeconds;
    }

    public TimeSpan GetRetryDelay(int failedAttempts)
    {
        if (failedAttempts <= 0) throw new ArgumentOutOfRangeException(nameof(failedAttempts));
        return TimeSpan.FromSeconds(Math.Min(MaxRetryDelaySeconds,
            InitialRetryDelaySeconds * Math.Pow(2, Math.Min(failedAttempts - 1, 30))));
    }
}
