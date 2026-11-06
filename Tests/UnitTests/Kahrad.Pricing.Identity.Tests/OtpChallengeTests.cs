using Kahrad.Pricing.Identity.Domain.Entities;
using Xunit;

namespace Kahrad.Pricing.Identity.Tests;

public sealed class OtpChallengeTests
{
    private static readonly DateTime Now = new(2026, 11, 6, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Correct_code_can_be_consumed_only_once()
    {
        var challenge = Create("123456");

        Assert.True(challenge.Verify("123456", Now.AddMinutes(1)));
        challenge.Consume(Now.AddMinutes(1));

        Assert.False(challenge.IsUsable(Now.AddMinutes(1)));
        Assert.False(challenge.Verify("123456", Now.AddMinutes(1)));
    }

    [Fact]
    public void Wrong_codes_invalidate_challenge_at_attempt_limit()
    {
        var challenge = Create("123456", maxAttempts: 3);

        Assert.False(challenge.Verify("111111", Now));
        Assert.False(challenge.Verify("222222", Now));
        Assert.False(challenge.Verify("333333", Now));

        Assert.Equal(3, challenge.FailedAttempts);
        Assert.False(challenge.IsUsable(Now));
    }

    [Fact]
    public void Expired_code_is_rejected_without_incrementing_attempts()
    {
        var challenge = Create("123456");

        Assert.False(challenge.Verify("123456", Now.AddMinutes(5)));
        Assert.Equal(0, challenge.FailedAttempts);
    }

    [Fact]
    public void Stored_values_do_not_contain_plaintext_code()
    {
        var challenge = Create("123456");

        Assert.DoesNotContain("123456", challenge.CodeHash, StringComparison.Ordinal);
        Assert.DoesNotContain("123456", challenge.CodeSalt, StringComparison.Ordinal);
    }

    private static OtpChallenge Create(string code, int maxAttempts = 5) =>
        OtpChallenge.Create(
            Guid.NewGuid(),
            code,
            Now,
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(1),
            maxAttempts);
}
