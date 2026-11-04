namespace Kahrad.Pricing.Sms.Application.Exceptions;

public class SmsOperationException : Exception
{
    public SmsOperationException(string message) : base(message)
    {
    }

    public SmsOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
