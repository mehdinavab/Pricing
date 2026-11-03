using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kahrad.Pricing.Identity.Domain.Entities
{
    [Table("RefreshTokens", Schema = "identity")]
    [Index(nameof(PublicId), IsUnique = true)]
    [Index(nameof(TokenHash), IsUnique = true)]
    [Index(nameof(UserId), nameof(ExpiresAtUtc))]
    [Index(nameof(FamilyId))]
    [Index(nameof(ReplacedByTokenId))]
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; private set; }

        public string TokenHash { get; private set; } = null!;

        public Guid FamilyId { get; private set; }

        public DateTime ExpiresAtUtc { get; private set; }

        public DateTime? RevokedAtUtc { get; private set; }

        public long? ReplacedByTokenId { get; private set; }

        public string? CreatedByIp { get; private set; }

        public string? RevokedByIp { get; private set; }

        public bool IsRevoked => RevokedAtUtc.HasValue;

        private RefreshToken()
        {
        }

        public RefreshToken(
            Guid userId,
            string tokenHash,
            Guid familyId,
            DateTime expiresAtUtc,
            string? createdByIp = null)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.", nameof(userId));

            ArgumentException.ThrowIfNullOrWhiteSpace(tokenHash);

            if (familyId == Guid.Empty)
                throw new ArgumentException("FamilyId cannot be empty.", nameof(familyId));

            UserId = userId;
            TokenHash = tokenHash;
            FamilyId = familyId;
            ExpiresAtUtc = expiresAtUtc;
            CreatedByIp = createdByIp;
        }

        public bool IsExpired(DateTime utcNow)
        {
            return utcNow >= ExpiresAtUtc;
        }

        public bool IsActive(DateTime utcNow)
        {
            return !IsRevoked && !IsExpired(utcNow);
        }

        public void Revoke(DateTime utcNow, string? revokedByIp = null)
        {
            if (IsRevoked)
                return;

            RevokedAtUtc = utcNow;
            RevokedByIp = revokedByIp;
        }

        public void MarkAsReplaced(
            long replacementTokenId,
            DateTime utcNow,
            string? revokedByIp = null)
        {
            if (replacementTokenId <= 0)
                throw new ArgumentOutOfRangeException(nameof(replacementTokenId));

            ReplacedByTokenId = replacementTokenId;
            Revoke(utcNow, revokedByIp);
        }
    }
}
