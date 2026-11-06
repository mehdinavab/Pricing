using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using System.Security.Cryptography;

namespace Kahrad.Pricing.Identity.Domain.Entities;

public sealed class OtpChallenge : BaseEntity
{
    private const int HashSize = 32;
    private const int SaltSize = 16;
    private const int HashIterations = 100_000;

    private OtpChallenge()
    {
    }

    private OtpChallenge(
        Guid userId,
        string code,
        DateTime createdAtUtc,
        DateTime expiresAtUtc,
        DateTime nextRequestAtUtc,
        int maxAttempts)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID is required.", nameof(userId));
        if (expiresAtUtc <= createdAtUtc)
            throw new ArgumentOutOfRangeException(nameof(expiresAtUtc));
        if (nextRequestAtUtc <= createdAtUtc || nextRequestAtUtc > expiresAtUtc)
            throw new ArgumentOutOfRangeException(nameof(nextRequestAtUtc));
        if (maxAttempts <= 0)
            throw new ArgumentOutOfRangeException(nameof(maxAttempts));

        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        UserId = userId;
        CodeSalt = Convert.ToBase64String(salt);
        CodeHash = HashCode(code, salt);
        ExpiresAtUtc = expiresAtUtc;
        NextRequestAtUtc = nextRequestAtUtc;
        MaxAttempts = maxAttempts;
        SetCreated(null, createdAtUtc);
    }

    public Guid UserId { get; private set; }
    public string CodeHash { get; private set; } = null!;
    public string CodeSalt { get; private set; } = null!;
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime NextRequestAtUtc { get; private set; }
    public int FailedAttempts { get; private set; }
    public int MaxAttempts { get; private set; }
    public DateTime? ConsumedAtUtc { get; private set; }
    public DateTime? InvalidatedAtUtc { get; private set; }

    public bool IsUsable(DateTime utcNow) =>
        ConsumedAtUtc is null &&
        InvalidatedAtUtc is null &&
        FailedAttempts < MaxAttempts &&
        utcNow < ExpiresAtUtc;

    public static OtpChallenge Create(
        Guid userId,
        string code,
        DateTime createdAtUtc,
        TimeSpan lifetime,
        TimeSpan resendCooldown,
        int maxAttempts) =>
        new(
            userId,
            code,
            createdAtUtc,
            createdAtUtc.Add(lifetime),
            createdAtUtc.Add(resendCooldown),
            maxAttempts);

    public bool Verify(string code, DateTime utcNow)
    {
        if (!IsUsable(utcNow) || string.IsNullOrWhiteSpace(code))
            return false;

        var actualHash = Convert.FromBase64String(CodeHash);
        var candidateHash = Convert.FromBase64String(HashCode(
            code,
            Convert.FromBase64String(CodeSalt)));
        var matches = CryptographicOperations.FixedTimeEquals(actualHash, candidateHash);
        if (matches)
            return true;

        FailedAttempts++;
        if (FailedAttempts >= MaxAttempts)
            InvalidatedAtUtc = utcNow;
        SetUpdated(null, utcNow);
        return false;
    }

    public void Consume(DateTime utcNow)
    {
        if (!IsUsable(utcNow))
            throw new InvalidOperationException("OTP challenge is no longer usable.");

        ConsumedAtUtc = utcNow;
        SetUpdated(null, utcNow);
    }

    public void Invalidate(DateTime utcNow)
    {
        if (ConsumedAtUtc is not null || InvalidatedAtUtc is not null)
            return;

        InvalidatedAtUtc = utcNow;
        SetUpdated(null, utcNow);
    }

    private static string HashCode(string code, byte[] salt) =>
        Convert.ToBase64String(Rfc2898DeriveBytes.Pbkdf2(
            code,
            salt,
            HashIterations,
            HashAlgorithmName.SHA256,
            HashSize));
}
