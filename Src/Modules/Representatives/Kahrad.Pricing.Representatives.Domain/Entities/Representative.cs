using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.Representatives.Domain.Entities;

public sealed class Representative : BaseEntity
{
    private Representative() { }
    public Representative(long brandId, string code, string name)
    {
        if (brandId <= 0) throw new ArgumentOutOfRangeException(nameof(brandId));
        ArgumentException.ThrowIfNullOrWhiteSpace(code);
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        if (code.Length > 100 || name.Length > 250) throw new ArgumentException("Representative details are too long.");
        BrandId = brandId; Code = code.Trim().ToUpperInvariant(); Name = name.Trim();
    }
    public long BrandId { get; private set; }
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; } = true;
    public void SetActive(bool active) => IsActive = active;
}
