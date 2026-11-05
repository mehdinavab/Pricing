using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.AccountingIntegration.Domain.Entities;

public sealed class AccountingRepresentative : BaseEntity
{
    private AccountingRepresentative() { }
    public AccountingRepresentative(long brandId, string externalRepresentativeId, string name, string? code, DateTime seenAtUtc)
    {
        if (brandId <= 0) throw new ArgumentOutOfRangeException(nameof(brandId));
        ArgumentException.ThrowIfNullOrWhiteSpace(externalRepresentativeId);
        BrandId = brandId; ExternalRepresentativeId = externalRepresentativeId.Trim();
        Update(name, code, seenAtUtc);
    }

    public long BrandId { get; private set; }
    public string ExternalRepresentativeId { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Code { get; private set; }
    public DateTime LastSeenAtUtc { get; private set; }
    public bool IsPresentInLatestSync { get; private set; } = true;

    public void Update(string name, string? code, DateTime seenAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Length > 250 || code?.Length > 100) throw new ArgumentException("Accounting representative details are too long.");
        Name = name.Trim(); Code = code?.Trim(); LastSeenAtUtc = seenAtUtc;
        IsPresentInLatestSync = true;
    }
    public void MarkAbsent() => IsPresentInLatestSync = false;
}
