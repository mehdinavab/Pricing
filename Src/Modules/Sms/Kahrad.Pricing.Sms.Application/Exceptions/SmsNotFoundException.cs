namespace Kahrad.Pricing.Sms.Application.Exceptions;

public sealed class SmsNotFoundException : KeyNotFoundException
{
    public SmsNotFoundException(long id)
        : base($"SMS message with ID '{id}' was not found.")
    {
    }

    public SmsNotFoundException(Guid publicId)
        : base($"SMS message with Public ID '{publicId}' was not found.")
    {
    }
}
