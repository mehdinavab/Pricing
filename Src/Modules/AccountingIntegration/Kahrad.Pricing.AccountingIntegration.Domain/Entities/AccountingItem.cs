using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.AccountingIntegration.Domain.Entities;

public sealed class AccountingItem : BaseEntity
{
    private AccountingItem() { }
    public AccountingItem(long brandId, string externalItemId, string name, string? code, string? unitCode, DateTime seenAtUtc)
    {
        if (brandId <= 0) throw new ArgumentOutOfRangeException(nameof(brandId));
        ArgumentException.ThrowIfNullOrWhiteSpace(externalItemId);
        BrandId = brandId;
        ExternalItemId = externalItemId.Trim();
        Update(name, code, unitCode, seenAtUtc);
    }

    public long BrandId { get; private set; }
    public string ExternalItemId { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public string? Code { get; private set; }
    public string? UnitCode { get; private set; }
    public DateTime LastSeenAtUtc { get; private set; }
    public bool IsPresentInLatestSync { get; private set; } = true;

    public void Update(string name, string? code, string? unitCode, DateTime seenAtUtc)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (name.Length > 250 || code?.Length > 100 || unitCode?.Length > 32)
            throw new ArgumentException("Accounting item details are too long.");
        Name = name.Trim(); Code = code?.Trim(); UnitCode = unitCode?.Trim();
        LastSeenAtUtc = seenAtUtc; IsPresentInLatestSync = true;
    }

    public void MarkAbsent() => IsPresentInLatestSync = false;
}
