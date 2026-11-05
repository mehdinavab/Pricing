using Kahrad.Pricing.BuildingBlocks.Domain.Common;

namespace Kahrad.Pricing.Representatives.Domain.Entities;

public sealed class RepresentativeAccountingMapping : BaseEntity
{
    private RepresentativeAccountingMapping() { }
    public RepresentativeAccountingMapping(long brandId, long representativeId, long accountingRepresentativeId, Guid? mappedByUserId)
    {
        if (brandId <= 0 || representativeId <= 0 || accountingRepresentativeId <= 0)
            throw new ArgumentOutOfRangeException(nameof(representativeId));
        BrandId = brandId; RepresentativeId = representativeId;
        AccountingRepresentativeId = accountingRepresentativeId; MappedByUserId = mappedByUserId;
    }
    public long BrandId { get; private set; }
    public long RepresentativeId { get; private set; }
    public long AccountingRepresentativeId { get; private set; }
    public Guid? MappedByUserId { get; private set; }
    public bool IsActive { get; private set; } = true;
    public void Deactivate() => IsActive = false;
}
