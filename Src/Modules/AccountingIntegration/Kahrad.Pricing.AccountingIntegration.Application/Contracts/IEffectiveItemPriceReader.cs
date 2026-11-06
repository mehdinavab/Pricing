namespace Kahrad.Pricing.AccountingIntegration.Application.Contracts;

public sealed record EffectiveItemPrice(long LocalItemId, long BrandId, decimal Amount,
    string CurrencyCode, decimal UnitConversionFactor, long SourceSyncRunId);

public interface IEffectiveItemPriceReader
{
    Task<IReadOnlyList<EffectiveItemPrice>> GetAsync(long brandId,
        IReadOnlyCollection<long> localItemIds, CancellationToken cancellationToken);
}
