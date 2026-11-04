using Kahrad.Pricing.Identity.Domain.Entities;

namespace Kahrad.Pricing.Identity.Application.Abstractions.Persistence
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByTokenHashAsync(
            string tokenHash,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyCollection<RefreshToken>> GetActiveByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            RefreshToken refreshToken,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            RefreshToken refreshToken,
            CancellationToken cancellationToken = default);

        Task ReplaceAsync(
            RefreshToken currentToken,
            RefreshToken replacementToken,
            DateTime replacedAtUtc,
            CancellationToken cancellationToken = default);

        Task RevokeAllByUserIdAsync(
            Guid userId,
            DateTime revokedAtUtc,
            CancellationToken cancellationToken = default);

        Task RevokeFamilyAsync(
            Guid familyId,
            DateTime revokedAtUtc,
            CancellationToken cancellationToken = default);
    }
}
