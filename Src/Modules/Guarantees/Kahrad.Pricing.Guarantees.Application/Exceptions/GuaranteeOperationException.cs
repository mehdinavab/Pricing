namespace Kahrad.Pricing.Guarantees.Application.Exceptions;

public class GuaranteeOperationException : Exception
{
    public GuaranteeOperationException(string message) : base(message)
    {
    }

    public GuaranteeOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
