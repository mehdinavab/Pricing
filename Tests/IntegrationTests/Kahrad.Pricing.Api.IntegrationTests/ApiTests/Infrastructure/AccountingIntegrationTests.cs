using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.AccountingIntegration.Domain.Entities;
using Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;
using Kahrad.Pricing.Api.Controllers.Accounting;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Products.Domain.Entities;
using Kahrad.Pricing.Products.Infrastructure.Persistence;
using Kahrad.Pricing.Sales.Infrastructure.Persistence;
using Kahrad.Pricing.Brands.Domain.Entities;
using Kahrad.Pricing.Brands.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Infrastructure;

public sealed class AccountingIntegrationTests(KahradPricingApiWebAppFactory factory) : ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Connection_metadata_never_returns_a_secret_reference_or_enables_provider()
    {
        long brandId;
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<BrandsDbContext>();
            var brand = new Brand("Brand", "CONNECTION-TEST");
            db.Brands.Add(brand); await db.SaveChangesAsync(); brandId = brand.Id;
        }
        using var saved = await Client.PostAsJsonAsync("/api/accounting/connections/save",
            new SaveAccountingConnectionRequest(brandId, "https://accounting.example.test", "env:ACCOUNTING_TEST_KEY"));
        Assert.Equal(HttpStatusCode.NoContent, saved.StatusCode);
        using var listed = await Client.PostAsJsonAsync("/api/accounting/connections/list", new { });
        Assert.Equal(HttpStatusCode.OK, listed.StatusCode);
        var body = await listed.Content.ReadAsStringAsync();
        Assert.DoesNotContain("ACCOUNTING_TEST_KEY", body);
        Assert.Contains("hasSecretReference", body);
        Assert.Contains("isEnabled", body);
        using var sync = await Client.PostAsJsonAsync("/api/accounting/sync/items", new StartAccountingSyncRequest(brandId));
        Assert.Equal(HttpStatusCode.ServiceUnavailable, sync.StatusCode);
    }

    [SqlServerFact]
    public async Task Invoice_sync_updates_only_the_external_mirror()
    {
        await AddConnectionAsync(1, "test:brand-one");
        Factory.AccountingProvider.IsConfigured = true;
        Factory.AccountingProvider.Invoices = new([
            new ImportedAccountingInvoice("INV-1", "R-1", "100", "Draft", DateTime.UtcNow)], true);
        using var first = await Client.PostAsJsonAsync("/api/accounting/sync/invoices", new StartAccountingSyncRequest(1));
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Factory.AccountingProvider.Invoices = new([
            new ImportedAccountingInvoice("INV-1", "R-1", "100", "Updated", DateTime.UtcNow)], true);
        using var repeated = await Client.PostAsJsonAsync("/api/accounting/sync/invoices", new StartAccountingSyncRequest(1));
        Assert.Equal(HttpStatusCode.OK, repeated.StatusCode);
        Assert.Equal(1, await ReadDatabaseAsync<AccountingDbContext,int>(db => db.Invoices.CountAsync()));
        Assert.Equal("Updated", await ReadDatabaseAsync<AccountingDbContext,string>(db =>
            db.Invoices.Select(x => x.ExternalStatus!).SingleAsync()));
        Assert.Equal(0, await ReadDatabaseAsync<SalesDbContext,int>(db => db.SoldProductUnits.CountAsync()));
    }

    private async Task AddConnectionAsync(long brandId, string reference)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AccountingDbContext>();
        var connection = new AccountingBrandConnection(brandId, "TEST");
        connection.Configure("https://accounting.example.test", reference, true);
        db.Connections.Add(connection);
        await db.SaveChangesAsync();
    }

    [SqlServerFact]
    public async Task Unconfigured_provider_returns_503_without_network_call()
    {
        using var response = await Client.PostAsJsonAsync("/api/accounting/sync/items", new StartAccountingSyncRequest(1));
        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
        Assert.Empty(Factory.AccountingProvider.Calls);
    }

    [SqlServerFact]
    public async Task Item_sync_is_brand_scoped_idempotent_and_preserves_price_history()
    {
        await AddConnectionAsync(1, "test:brand-one");
        await AddConnectionAsync(2, "test:brand-two");
        Factory.AccountingProvider.IsConfigured = true;
        Factory.AccountingProvider.Items = new([new ImportedAccountingItem("A1", "External item", "E1", "PCS", 100m, "IRR")], true);
        using var first = await Client.PostAsJsonAsync("/api/accounting/sync/items", new StartAccountingSyncRequest(1));
        using var repeated = await Client.PostAsJsonAsync("/api/accounting/sync/items", new StartAccountingSyncRequest(1));
        using var secondBrand = await Client.PostAsJsonAsync("/api/accounting/sync/items", new StartAccountingSyncRequest(2));
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(HttpStatusCode.OK, repeated.StatusCode);
        Assert.Equal(HttpStatusCode.OK, secondBrand.StatusCode);
        Assert.Equal(2, await ReadDatabaseAsync<AccountingDbContext,int>(db => db.Items.CountAsync()));
        Assert.Equal(2, await ReadDatabaseAsync<AccountingDbContext,int>(db => db.ItemPrices.CountAsync()));
        Assert.Equal(new[] { "test:brand-one", "test:brand-one", "test:brand-two" },
            Factory.AccountingProvider.Calls.Select(x => x.ApiKeySecretReference));

        Factory.AccountingProvider.Items = new([new ImportedAccountingItem("A1", "External item", "E1", "PCS", 120m, "IRR")], true);
        using var changed = await Client.PostAsJsonAsync("/api/accounting/sync/items", new StartAccountingSyncRequest(1));
        Assert.Equal(HttpStatusCode.OK, changed.StatusCode);
        Assert.Equal(new[] { 100m, 120m }, await ReadDatabaseAsync<AccountingDbContext,decimal[]>(db =>
            db.ItemPrices.Where(x => x.BrandId == 1).OrderBy(x => x.Id).Select(x => x.Amount).ToArrayAsync()));
    }

    [SqlServerFact]
    public async Task Incomplete_sync_does_not_mark_items_absent()
    {
        await AddConnectionAsync(1, "test:brand-one");
        Factory.AccountingProvider.IsConfigured = true;
        Factory.AccountingProvider.Items = new([new ImportedAccountingItem("A1", "External item", null, "PCS", null, null)], true);
        using var first = await Client.PostAsJsonAsync("/api/accounting/sync/items", new StartAccountingSyncRequest(1));
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Factory.AccountingProvider.Items = new([], false);
        using var partial = await Client.PostAsJsonAsync("/api/accounting/sync/items", new StartAccountingSyncRequest(1));
        Assert.Equal(HttpStatusCode.InternalServerError, partial.StatusCode);
        Assert.True(await ReadDatabaseAsync<AccountingDbContext,bool>(db =>
            db.Items.Where(x => x.BrandId == 1).Select(x => x.IsPresentInLatestSync).SingleAsync()));
        Assert.Equal(AccountingSyncStatus.Failed, await ReadDatabaseAsync<AccountingDbContext,AccountingSyncStatus>(db =>
            db.SyncRuns.OrderByDescending(x => x.Id).Select(x => x.Status).FirstAsync()));
        using var history = await Client.PostAsJsonAsync("/api/accounting/sync/history",
            new AccountingSyncHistoryRequest(1, AccountingSyncType.Items));
        Assert.Equal(HttpStatusCode.OK, history.StatusCode);
        var runs = await history.Content.ReadFromJsonAsync<AccountingSyncRunView[]>();
        Assert.NotNull(runs);
        Assert.Equal(2, runs.Length);
        Assert.Contains(runs, x => x.Status == AccountingSyncStatus.Succeeded && x.FullScanCompleted);
        Assert.Contains(runs, x => x.Status == AccountingSyncStatus.Failed && !x.FullScanCompleted);
    }

    [SqlServerFact]
    public async Task Mapping_rejects_other_brand_and_is_idempotent()
    {
        long localId;
        long externalId;
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var products = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
            var accounting = scope.ServiceProvider.GetRequiredService<AccountingDbContext>();
            var local = new Item("I1", "Local item", "PCS");
            products.Items.Add(local); await products.SaveChangesAsync(); localId = local.Id;
            var external = new AccountingItem(1, "A1", "External item", null, "PCS", DateTime.UtcNow);
            accounting.Items.Add(external); await accounting.SaveChangesAsync(); externalId = external.Id;
        }
        var request = new MapAccountingItemRequest(1, localId, externalId);
        using var first = await Client.PostAsJsonAsync("/api/accounting/items/map", request);
        using var repeated = await Client.PostAsJsonAsync("/api/accounting/items/map", request);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(await first.Content.ReadFromJsonAsync<long>(), await repeated.Content.ReadFromJsonAsync<long>());
        using var otherBrand = await Client.PostAsJsonAsync("/api/accounting/items/map", request with { BrandId = 2 });
        Assert.Equal(HttpStatusCode.BadRequest, otherBrand.StatusCode);
        Assert.Equal(1, await ReadDatabaseAsync<AccountingDbContext,int>(db => db.ItemMappings.CountAsync()));
    }
}
