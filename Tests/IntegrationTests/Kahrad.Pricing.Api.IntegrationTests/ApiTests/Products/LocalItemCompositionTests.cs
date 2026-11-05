using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.Controllers.Products.Items;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Products.Infrastructure.Persistence;
using Kahrad.Pricing.Sales.Application.Contracts;
using Kahrad.Pricing.Sales.Domain.Entities;
using Kahrad.Pricing.Sales.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Products;

public sealed class LocalItemCompositionTests(KahradPricingApiWebAppFactory factory) : ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Changing_current_composition_keeps_finalized_sale_snapshot()
    {
        var product = await Factory.CreateProductAsync();
        using var created = await Client.PostAsJsonAsync("/api/products/items/create",
            new CreateLocalItemRequest("ITEM-1", "Local component", "PCS"));
        Assert.Equal(HttpStatusCode.OK, created.StatusCode);
        var itemId = await created.Content.ReadFromJsonAsync<long>();
        using var composition = await Client.PostAsJsonAsync("/api/products/items/composition/replace",
            new ProductCompositionRequest(product.Id, [new ProductItemInput(itemId, 2m)]));
        Assert.Equal(HttpStatusCode.NoContent, composition.StatusCode);
        Assert.Equal(2m, await ReadDatabaseAsync<ProductsDbContext,decimal>(db =>
            db.ProductItems.Select(x => x.QuantityUsed).SingleAsync()));

        var sold = new SoldProductUnit(Guid.NewGuid(), 1, product.Id, 1, 100m, 150m, 170m, "IRR", 1, DateTime.UtcNow);
        await using (var scope = Factory.Services.CreateAsyncScope())
            await scope.ServiceProvider.GetRequiredService<ISaleFinalizationStore>()
                .SaveAtomicallyAsync(sold, new AccountingSalePostOutbox(sold), default);

        using var changed = await Client.PostAsJsonAsync("/api/products/items/composition/replace",
            new ProductCompositionRequest(product.Id, [new ProductItemInput(itemId, 3m)]));
        Assert.Equal(HttpStatusCode.NoContent, changed.StatusCode);
        Assert.Equal(3m, await ReadDatabaseAsync<ProductsDbContext,decimal>(db =>
            db.ProductItems.Select(x => x.QuantityUsed).SingleAsync()));
        Assert.Equal(170m, await ReadDatabaseAsync<SalesDbContext,decimal>(db =>
            db.SoldProductUnits.Select(x => x.CustomerSellingPriceSnapshot).SingleAsync()));
    }
}
