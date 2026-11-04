namespace Kahrad.Pricing.Brands.Application.Exceptions;

public class BrandOperationException : Exception
{
    public BrandOperationException(string message) : base(message)
    {
    }

    public BrandOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
