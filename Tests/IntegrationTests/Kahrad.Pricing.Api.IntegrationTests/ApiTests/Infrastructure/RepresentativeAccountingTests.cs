using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.AccountingIntegration.Domain.Entities;
using Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;
using Kahrad.Pricing.Api.Controllers.Accounting;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Representatives.Domain.Entities;
using Kahrad.Pricing.Representatives.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Infrastructure;

public sealed class RepresentativeAccountingTests(KahradPricingApiWebAppFactory factory) : ApiTestBase(factory)
{
    private async Task<long> AddExternalAsync(long brandId = 1)
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<AccountingDbContext>();
        var record = new AccountingRepresentative(brandId, "E-1", "Imported representative", "EXT", DateTime.UtcNow);
        db.Representatives.Add(record); await db.SaveChangesAsync();
        return record.Id;
    }

    [SqlServerFact]
    public async Task Import_is_idempotent_and_brand_scoped()
    {
        var externalId = await AddExternalAsync();
        var request = new ImportAccountingRepresentativeRequest(1, externalId, "LOCAL-1");
        using var first = await Client.PostAsJsonAsync("/api/representatives/accounting/import", request);
        using var repeated = await Client.PostAsJsonAsync("/api/representatives/accounting/import", request);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(await first.Content.ReadFromJsonAsync<long>(), await repeated.Content.ReadFromJsonAsync<long>());
        Assert.Equal(1, await ReadDatabaseAsync<RepresentativesDbContext,int>(db => db.Representatives.CountAsync()));
        Assert.Equal(1, await ReadDatabaseAsync<RepresentativesDbContext,int>(db => db.AccountingMappings.CountAsync()));
        using var wrongBrand = await Client.PostAsJsonAsync("/api/representatives/accounting/import", request with { BrandId = 2 });
        Assert.Equal(HttpStatusCode.BadRequest, wrongBrand.StatusCode);
    }

    [SqlServerFact]
    public async Task Explicit_match_does_not_replace_local_representative()
    {
        var externalId = await AddExternalAsync();
        long representativeId;
        await using (var scope = Factory.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<RepresentativesDbContext>();
            var local = new Representative(1, "LOCAL", "Local representative");
            db.Representatives.Add(local); await db.SaveChangesAsync(); representativeId = local.Id;
        }
        var request = new MatchAccountingRepresentativeRequest(1, representativeId, externalId);
        using var first = await Client.PostAsJsonAsync("/api/representatives/accounting/map", request);
        using var repeated = await Client.PostAsJsonAsync("/api/representatives/accounting/map", request);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);
        Assert.Equal(await first.Content.ReadFromJsonAsync<long>(), await repeated.Content.ReadFromJsonAsync<long>());
        Assert.Equal("Local representative", await ReadDatabaseAsync<RepresentativesDbContext,string>(db =>
            db.Representatives.Select(x => x.Name).SingleAsync()));
        Assert.Equal(1, await ReadDatabaseAsync<RepresentativesDbContext,int>(db => db.Representatives.CountAsync()));
    }
}
