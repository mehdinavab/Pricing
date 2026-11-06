namespace Kahrad.Pricing.Pricing.Engine;

public enum PricingCalculationErrorCode
{
    ProductHasNoComponents,
    BrandMismatch,
    DuplicateItem,
    ItemPriceMissing,
    InvalidItemPrice,
    CurrencyMismatch,
    InvalidQuantity,
    UnitConversionMissing,
    InvalidUnitConversion,
    InvalidFactoryPolicy,
    FixedPriceRequired,
    InvalidPriceAdjustment,
    InvalidWarrantyAmount,
    InvalidRepresentativePolicy,
    DiscountBelowPurchasePrice,
    InvalidRoundingStep,
    DecimalOverflow
}

public sealed class PricingCalculationException : InvalidOperationException
{
    public PricingCalculationException(PricingCalculationErrorCode errorCode, string message, long? itemId = null)
        : base(message)
    {
        ErrorCode = errorCode;
        ItemId = itemId;
    }

    public PricingCalculationException(PricingCalculationErrorCode errorCode, string message, OverflowException innerException)
        : base(message, innerException) => ErrorCode = errorCode;

    public PricingCalculationErrorCode ErrorCode { get; }
    public long? ItemId { get; }
}
