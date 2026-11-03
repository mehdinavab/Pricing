using Kahrad.Pricing.Identity.Application.Abstractions.Persistence;
using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Identity.Infrastructure.Persistence.Repositories;

public sealed class RefreshTokenRepository(IdentityDbContext context)
    : IRefreshTokenRepository
{
    public async Task AddAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        await context.RefreshTokens.AddAsync(refreshToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<RefreshToken>> GetActiveByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        return await context.RefreshTokens
            .AsNoTracking()
            .Where(x =>
                x.UserId == userId &&
                x.RevokedAtUtc == null &&
                x.ExpiresAtUtc > now)
            .OrderByDescending(x => x.ExpiresAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(
        string tokenHash,
        CancellationToken cancellationToken = default)
    {
        return await context.RefreshTokens
            .FirstOrDefaultAsync(
                x => x.TokenHash == tokenHash,
                cancellationToken);
    }

    public async Task RevokeAllByUserIdAsync(
        Guid userId,
        DateTime revokedAtUtc,
        CancellationToken cancellationToken = default)
    {
        await context.RefreshTokens
            .Where(x =>
                x.UserId == userId &&
                x.RevokedAtUtc == null)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    x => x.RevokedAtUtc,
                    revokedAtUtc),
                cancellationToken);
    }

    public async Task RevokeFamilyAsync(
        Guid familyId,
        DateTime revokedAtUtc,
        CancellationToken cancellationToken = default)
    {
        await context.RefreshTokens
            .Where(x =>
                x.FamilyId == familyId &&
                x.RevokedAtUtc == null)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    x => x.RevokedAtUtc,
                    revokedAtUtc),
                cancellationToken);
    }

    public async Task UpdateAsync(
        RefreshToken refreshToken,
        CancellationToken cancellationToken = default)
    {
        context.RefreshTokens.Update(refreshToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReplaceAsync(
        RefreshToken currentToken,
        RefreshToken replacementToken,
        DateTime replacedAtUtc,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(
            cancellationToken);

        await context.RefreshTokens.AddAsync(replacementToken, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        currentToken.MarkAsReplaced(replacementToken.Id, replacedAtUtc);
        context.RefreshTokens.Update(currentToken);
        await context.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }
}