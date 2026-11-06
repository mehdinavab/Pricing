using Kahrad.Pricing.Pricing.Application;
using Kahrad.Pricing.Pricing.Application.Contracts;
using Kahrad.Pricing.Pricing.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Pricing.Infrastructure.Persistence;

public sealed class PricingPolicyStore(PricingDbContext db) : IPricingPolicyStore
{
    public Task<BrandPricingPolicy?> GetBrandPolicyAsync(long brandId, CancellationToken ct) =>
        db.BrandPricingPolicies.SingleOrDefaultAsync(x => x.BrandId == brandId, ct);

    public Task<RepresentativePricingPolicy?> GetRepresentativePolicyAsync(long brandId,
        long representativeId, CancellationToken ct) =>
        db.RepresentativePricingPolicies.SingleOrDefaultAsync(x =>
            x.BrandId == brandId && x.RepresentativeId == representativeId, ct);

    public Task<RepresentativeProductDiscount?> GetProductDiscountAsync(long brandId,
        long representativeId, long productId, CancellationToken ct) =>
        db.RepresentativeProductDiscounts.SingleOrDefaultAsync(x =>
            x.BrandId == brandId && x.RepresentativeId == representativeId &&
            x.ProductId == productId, ct);

    public async Task<IReadOnlyList<RepresentativeProductDiscount>> GetProductDiscountsAsync(
        long brandId, long representativeId, CancellationToken ct) =>
        await db.RepresentativeProductDiscounts.Where(x => x.BrandId == brandId &&
            x.RepresentativeId == representativeId).ToListAsync(ct);

    public Task SaveAsync(BrandPricingPolicy policy, CancellationToken ct) => SaveEntityAsync(policy, ct);
    public Task SaveAsync(RepresentativePricingPolicy policy, CancellationToken ct) => SaveEntityAsync(policy, ct);
    public Task SaveAsync(RepresentativeProductDiscount discount, CancellationToken ct) => SaveEntityAsync(discount, ct);

    private async Task SaveEntityAsync<TEntity>(TEntity entity, CancellationToken ct)
        where TEntity : Kahrad.Pricing.BuildingBlocks.Domain.Common.BaseEntity
    {
        if (entity.Id == 0) db.Set<TEntity>().Add(entity);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException)
        { throw new PricingOperationException("Pricing policy changed; reload before editing.", true); }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        { throw new PricingOperationException("Pricing policy already exists or changed concurrently.", true); }
    }
}
