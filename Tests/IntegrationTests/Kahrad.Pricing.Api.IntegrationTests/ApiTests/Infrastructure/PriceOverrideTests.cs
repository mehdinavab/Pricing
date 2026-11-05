using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.Controllers.Accounting;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Brands.Domain.Entities;
using Kahrad.Pricing.Brands.Infrastructure.Persistence;
using Kahrad.Pricing.Pricing.Domain.Entities;
using Kahrad.Pricing.Pricing.Infrastructure.Persistence;
using Kahrad.Pricing.Products.Domain.Entities;
using Kahrad.Pricing.Products.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Infrastructure;

public sealed class PriceOverrideTests(KahradPricingApiWebAppFactory factory) : ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Fixed_override_and_manual_addition_do_not_modify_product_cost()
    {
        long brandId; long productId;
        var group = await Factory.CreateProductGroupAsync();
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var brands = scope.ServiceProvider.GetRequiredService<BrandsDbContext>();
            var brand = new Brand("Brand", "OVERRIDE-TEST");
            brands.Brands.Add(brand); await brands.SaveChangesAsync(); brandId = brand.Id;
            var products = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
            var product = new Product("Product", "OVERRIDE-SKU", group.Id, 100m, brandId);
            products.Products.Add(product); await products.SaveChangesAsync(); productId = product.Id;
        }
        var request = new SavePriceOverrideRequest(brandId, productId, FactorySellingPriceMode.Fixed, 125000m, 5000m, null);
        using var saved = await Client.PostAsJsonAsync("/api/pricing/product-override/save", request);
        Assert.Equal(HttpStatusCode.OK, saved.StatusCode);
        var first = await saved.Content.ReadFromJsonAsync<PriceOverrideView>();
        Assert.NotNull(first);
        Assert.Equal(125000m, first.FixedFactorySellingPrice);
        Assert.Equal(5000m, first.ManualFactoryPriceAdjustment);
        Assert.Equal(100m, await ReadDatabaseAsync<ProductsDbContext,decimal>(db =>
            db.Products.Where(x => x.Id == productId).Select(x => x.BasePrice).SingleAsync()));
        using var stale = await Client.PostAsJsonAsync("/api/pricing/product-override/save", request);
        Assert.Equal(HttpStatusCode.Conflict, stale.StatusCode);
        using var changed = await Client.PostAsJsonAsync("/api/pricing/product-override/save", request with
        { FixedFactorySellingPrice = 130000m, RowVersion = first.RowVersion });
        Assert.Equal(HttpStatusCode.OK, changed.StatusCode);
        Assert.Equal(1, await ReadDatabaseAsync<PricingDbContext,int>(db => db.ProductFactoryPriceOverrides.CountAsync()));
    }
}
