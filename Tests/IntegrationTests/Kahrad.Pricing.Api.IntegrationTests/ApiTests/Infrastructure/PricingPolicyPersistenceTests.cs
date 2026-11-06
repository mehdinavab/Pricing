using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.AccountingIntegration.Domain.Entities;
using Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;
using Kahrad.Pricing.Pricing.Application.Services;
using Kahrad.Pricing.Pricing.Domain.Entities;
using Kahrad.Pricing.Pricing.Infrastructure.Persistence;
using Kahrad.Pricing.Products.Domain.Entities;
using Kahrad.Pricing.Products.Infrastructure.Persistence;
using Kahrad.Pricing.Representatives.Domain.Entities;
using Kahrad.Pricing.Representatives.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Infrastructure;

public sealed class PricingPolicyPersistenceTests(KahradPricingApiWebAppFactory factory)
    : ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Pricing_policies_preserve_precision_unique_scope_and_rowversion()
    {
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<PricingDbContext>();
            db.BrandPricingPolicies.Add(new BrandPricingPolicy(1, 10.123456m,
                15.654321m, 5.000001m, 1.12345678m));
            db.RepresentativePricingPolicies.Add(new RepresentativePricingPolicy(1, 10,
                20.123456m, 5.123456m));
            db.RepresentativeProductDiscounts.Add(new RepresentativeProductDiscount(1, 10,
                100, 3.123456m));
            await db.SaveChangesAsync();
        }

        await using var firstScope = Factory.Services.CreateAsyncScope();
        await using var secondScope = Factory.Services.CreateAsyncScope();
        var firstDb = firstScope.ServiceProvider.GetRequiredService<PricingDbContext>();
        var secondDb = secondScope.ServiceProvider.GetRequiredService<PricingDbContext>();
        var first = await firstDb.BrandPricingPolicies.SingleAsync();
        var second = await secondDb.BrandPricingPolicies.SingleAsync();
        Assert.Equal(10.123456m, first.BeautyPercentage);
        Assert.Equal(1.12345678m, first.RepresentativeFactor);

        first.Change(11m, 15.654321m, 5.000001m, 1.12345678m,
            5000m, null, DateTime.UtcNow);
        await firstDb.SaveChangesAsync();
        second.Change(12m, 15.654321m, 5.000001m, 1.12345678m,
            5000m, null, DateTime.UtcNow);
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => secondDb.SaveChangesAsync());
    }

    [SqlServerFact]
    public async Task Representative_product_discount_scope_is_unique()
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<PricingDbContext>();
        db.RepresentativeProductDiscounts.AddRange(
            new RepresentativeProductDiscount(1, 10, 100, 3m),
            new RepresentativeProductDiscount(1, 10, 100, 4m));
        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }

    [SqlServerFact]
    public async Task Preview_reads_brand_scoped_composition_mapping_price_and_policies()
    {
        long productId;
        long itemId;
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var products = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
            var group = new ProductGroup("Pricing group", "PRICING-GROUP");
            products.ProductGroups.Add(group);
            await products.SaveChangesAsync();
            var product = new Product("Pricing product", "PRICING-PRODUCT", group.Id,
                0m, brandId: 1);
            var item = new Item("PRICING-ITEM", "Pricing item", "EA");
            products.Products.Add(product);
            products.Items.Add(item);
            await products.SaveChangesAsync();
            products.ProductItems.Add(new ProductItem(product.Id, item.Id, 2m));
            await products.SaveChangesAsync();
            productId = product.Id;
            itemId = item.Id;
        }

        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var accounting = scope.ServiceProvider.GetRequiredService<AccountingDbContext>();
            var run = new AccountingSyncRun(1, AccountingSyncType.Items, DateTime.UtcNow);
            var externalItem = new AccountingItem(1, "EXT-1", "External item", "E1", "EA",
                DateTime.UtcNow);
            accounting.SyncRuns.Add(run);
            accounting.Items.Add(externalItem);
            await accounting.SaveChangesAsync();
            accounting.ItemPrices.Add(new AccountingItemPrice(1, externalItem.Id, 100000m,
                "IRR", run.Id, DateTime.UtcNow));
            accounting.ItemMappings.Add(new AccountingItemMapping(1, itemId,
                externalItem.Id, 1m, null));
            await accounting.SaveChangesAsync();

            var representatives = scope.ServiceProvider.GetRequiredService<RepresentativesDbContext>();
            representatives.Representatives.Add(new Representative(1, "REP-1", "Representative"));
            await representatives.SaveChangesAsync();
            var representativeId = await representatives.Representatives.Select(x => x.Id).SingleAsync();

            var pricing = scope.ServiceProvider.GetRequiredService<PricingDbContext>();
            pricing.BrandPricingPolicies.Add(new BrandPricingPolicy(1, 10m, 15m, 5m));
            pricing.RepresentativePricingPolicies.Add(new RepresentativePricingPolicy(1,
                representativeId, 20m, 5m));
            pricing.RepresentativeProductDiscounts.Add(new RepresentativeProductDiscount(1,
                representativeId, productId, 3m));
            await pricing.SaveChangesAsync();
        }

        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var representativeId = await scope.ServiceProvider
                .GetRequiredService<RepresentativesDbContext>().Representatives
                .Select(x => x.Id).SingleAsync();
            var result = await scope.ServiceProvider.GetRequiredService<ProductPricePreviewService>()
                .CalculateAsync(productId, representativeId, 50000m, default);
            Assert.Equal(200000m, result.ProductionCost);
            Assert.Equal(310000m, result.RepresentativePrice);
            Assert.Equal(350000m, result.CustomerSellingPrice);
        }
    }
}
