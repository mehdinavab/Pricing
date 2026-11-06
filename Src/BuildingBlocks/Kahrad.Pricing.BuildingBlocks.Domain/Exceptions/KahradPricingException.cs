namespace Kahrad.Pricing.BuildingBlocks.Domain.Exceptions;

/// <summary>
/// Represents a failure that can safely cross an application boundary.
/// </summary>
public class KahradPricingException : Exception
{
    public KahradPricingException(
        bool isSystemError,
        ErrorCodes errorCode = ErrorCodes.BadRequest)
    {
        IsSystemError = isSystemError;
        ErrorCode = errorCode;
    }

    public KahradPricingException(
        string? message,
        bool isSystemError,
        ErrorCodes errorCode = ErrorCodes.BadRequest)
        : base(message)
    {
        IsSystemError = isSystemError;
        ErrorCode = errorCode;
    }

    public KahradPricingException(
        string? message,
        Exception? innerException,
        bool isSystemError = true,
        ErrorCodes errorCode = ErrorCodes.SystemError)
        : base(message, innerException)
    {
        IsSystemError = isSystemError;
        ErrorCode = errorCode;
    }

    public bool IsSystemError { get; }

    public ErrorCodes ErrorCode { get; }
}
