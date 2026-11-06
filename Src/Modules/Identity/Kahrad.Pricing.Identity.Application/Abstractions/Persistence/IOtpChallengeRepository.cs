using Kahrad.Pricing.Identity.Domain.Entities;

namespace Kahrad.Pricing.Identity.Application.Abstractions.Persistence;

public interface IOtpChallengeRepository
{
    Task<OtpChallenge?> GetActiveByUserIdAsync(
        Guid userId,
        DateTime utcNow,
        CancellationToken cancellationToken = default);

    Task<OtpChallenge?> GetByPublicIdAsync(
        Guid publicId,
        CancellationToken cancellationToken = default);

    Task ReplaceActiveAsync(
        OtpChallenge? currentChallenge,
        OtpChallenge replacement,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        OtpChallenge challenge,
        CancellationToken cancellationToken = default);

    Task CompleteAsync(
        OtpChallenge challenge,
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default);
}
