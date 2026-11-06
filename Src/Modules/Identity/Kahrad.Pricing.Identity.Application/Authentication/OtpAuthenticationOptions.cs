namespace Kahrad.Pricing.Identity.Application.Authentication;

public sealed class OtpAuthenticationOptions
{
    public const string SectionName = "Identity:Otp";

    public int CodeLength { get; set; } = 6;
    public int LifetimeMinutes { get; set; } = 5;
    public int ResendCooldownSeconds { get; set; } = 60;
    public int MaxFailedAttempts { get; set; } = 5;

    public void Validate()
    {
        if (CodeLength is < 4 or > 10)
            throw new InvalidOperationException("Identity OTP code length must be between 4 and 10.");
        if (LifetimeMinutes is < 1 or > 15)
            throw new InvalidOperationException("Identity OTP lifetime must be between 1 and 15 minutes.");
        if (ResendCooldownSeconds is < 15 or > 300)
            throw new InvalidOperationException("Identity OTP resend cooldown must be between 15 and 300 seconds.");
        if (ResendCooldownSeconds >= LifetimeMinutes * 60)
            throw new InvalidOperationException("Identity OTP resend cooldown must be shorter than its lifetime.");
        if (MaxFailedAttempts is < 1 or > 10)
            throw new InvalidOperationException("Identity OTP attempts must be between 1 and 10.");
    }
}
