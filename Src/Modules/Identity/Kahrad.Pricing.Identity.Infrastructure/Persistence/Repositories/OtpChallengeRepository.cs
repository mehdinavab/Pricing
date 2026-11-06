using Kahrad.Pricing.Identity.Application.Abstractions.Persistence;
using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Identity.Infrastructure.Persistence.Repositories;

public sealed class OtpChallengeRepository(IdentityDbContext context)
    : IOtpChallengeRepository
{
    public Task<OtpChallenge?> GetActiveByUserIdAsync(
        Guid userId,
        DateTime utcNow,
        CancellationToken cancellationToken = default) =>
        context.OtpChallenges
            .Where(x =>
                x.UserId == userId &&
                x.ConsumedAtUtc == null &&
                x.InvalidatedAtUtc == null &&
                x.ExpiresAtUtc > utcNow)
            .OrderByDescending(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<OtpChallenge?> GetByPublicIdAsync(
        Guid publicId,
        CancellationToken cancellationToken = default) =>
        context.OtpChallenges.SingleOrDefaultAsync(
            x => x.PublicId == publicId,
            cancellationToken);

    public async Task ReplaceActiveAsync(
        OtpChallenge? currentChallenge,
        OtpChallenge replacement,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(
            cancellationToken);
        if (currentChallenge is not null)
            context.OtpChallenges.Update(currentChallenge);
        await context.OtpChallenges.AddAsync(replacement, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task UpdateAsync(
        OtpChallenge challenge,
        CancellationToken cancellationToken = default)
    {
        context.OtpChallenges.Update(challenge);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task CompleteAsync(
        OtpChallenge challenge,
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(
            cancellationToken);
        context.OtpChallenges.Update(challenge);
        await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }
}
