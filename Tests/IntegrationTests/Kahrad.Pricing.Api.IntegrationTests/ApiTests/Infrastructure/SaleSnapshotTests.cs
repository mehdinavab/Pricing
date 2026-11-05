using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Sales.Application.Contracts;
using Kahrad.Pricing.Sales.Domain.Entities;
using Kahrad.Pricing.Sales.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Infrastructure;

public sealed class SaleSnapshotTests(KahradPricingApiWebAppFactory factory) : ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Finalized_sale_and_posting_intent_are_atomic_and_idempotent()
    {
        var operationId = Guid.NewGuid();
        var original = new SoldProductUnit(operationId, 1, 2, 3, 100m, 150m, 170m, "IRR", 1, DateTime.UtcNow);
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var store = scope.ServiceProvider.GetRequiredService<ISaleFinalizationStore>();
            await store.SaveAtomicallyAsync(original, new AccountingSalePostOutbox(original), default);
        }
        var retry = new SoldProductUnit(operationId, 1, 2, 3, 999m, 999m, 999m, "IRR", 1, DateTime.UtcNow);
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var store = scope.ServiceProvider.GetRequiredService<ISaleFinalizationStore>();
            var result = await store.SaveAtomicallyAsync(retry, new AccountingSalePostOutbox(retry), default);
            Assert.Equal(original.PublicId, result.PublicId);
        }
        Assert.Equal(1, await ReadDatabaseAsync<SalesDbContext,int>(db => db.SoldProductUnits.CountAsync()));
        Assert.Equal(1, await ReadDatabaseAsync<SalesDbContext,int>(db => db.AccountingSalePosts.CountAsync()));
        Assert.Equal(100m, await ReadDatabaseAsync<SalesDbContext,decimal>(db =>
            db.SoldProductUnits.Select(x => x.ProductionCostSnapshot).SingleAsync()));
    }

    [SqlServerFact]
    public async Task Unknown_provider_outcome_requires_review_without_repricing_sale()
    {
        var sale = new SoldProductUnit(Guid.NewGuid(), 1, 2, 3, 100m, 150m, 170m, "IRR", 1, DateTime.UtcNow);
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var store = scope.ServiceProvider.GetRequiredService<ISaleFinalizationStore>();
            await store.SaveAtomicallyAsync(sale, new AccountingSalePostOutbox(sale), default);
        }
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<SalesDbContext>();
            var post = await db.AccountingSalePosts.SingleAsync();
            post.StartAttempt(DateTime.UtcNow);
            post.Fail("UnknownOutcome", true, false, DateTime.UtcNow, null, 3);
            await db.SaveChangesAsync();
        }
        Assert.Equal(AccountingSalePostStatus.RequiresManualReview,
            await ReadDatabaseAsync<SalesDbContext,AccountingSalePostStatus>(db =>
                db.AccountingSalePosts.Select(x => x.Status).SingleAsync()));
        Assert.Equal(170m, await ReadDatabaseAsync<SalesDbContext,decimal>(db =>
            db.SoldProductUnits.Select(x => x.CustomerSellingPriceSnapshot).SingleAsync()));
    }
}
