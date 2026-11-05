using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Brands.Application.DTOs.Brands;
using Kahrad.Pricing.Brands.Infrastructure.Persistence;
using Kahrad.Pricing.Identity.Domain.Constants;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Brands;
public sealed class BrandEndpointTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Create_update_and_soft_delete_persist_through_real_services()
    {
        using var created=await Client.PostAsJsonAsync("/api/brands",new CreateBrandRequest{Name="  Brand One  ",Code="brand-one"});
        Assert.Equal(HttpStatusCode.Created,created.StatusCode);var id=await ReadResultAsync<long>(created);
        var brand=await ReadDatabaseAsync<BrandsDbContext,Kahrad.Pricing.Brands.Domain.Entities.Brand>(db=>db.Brands.AsNoTracking().SingleAsync(x=>x.Id==id));
        Assert.Equal("Brand One",brand.Name);Assert.Equal("BRAND-ONE",brand.Code);
        using var updated=await Client.PutAsJsonAsync($"/api/brands/{id}",new UpdateBrandRequest{Name="Updated brand",Code="BRAND-ONE"});Assert.Equal(HttpStatusCode.NoContent,updated.StatusCode);
        using var details=await Client.GetAsync($"/api/brands/{id}");Assert.Equal(HttpStatusCode.OK,details.StatusCode);Assert.Equal("Updated brand",(await ReadResultAsync<BrandDto>(details)).Name);
        using var deleted=await Client.DeleteAsync($"/api/brands/{id}");Assert.Equal(HttpStatusCode.NoContent,deleted.StatusCode);
        Assert.True(await ReadDatabaseAsync<BrandsDbContext,bool>(db=>db.Brands.IgnoreQueryFilters().AnyAsync(x=>x.Id==id&&x.IsDeleted)));
        using var missing=await Client.GetAsync($"/api/brands/{id}");Assert.Equal(HttpStatusCode.NotFound,missing.StatusCode);
    }
    [SqlServerFact]
    public async Task Duplicate_brand_code_is_rejected()
    {
        var request=new CreateBrandRequest{Name="Brand",Code="DUPLICATE"};using var first=await Client.PostAsJsonAsync("/api/brands",request);Assert.Equal(HttpStatusCode.Created,first.StatusCode);
        using var duplicate=await Client.PostAsJsonAsync("/api/brands",request);Assert.Equal(HttpStatusCode.BadRequest,duplicate.StatusCode);Assert.Equal(1,await ReadDatabaseAsync<BrandsDbContext,int>(db=>db.Brands.CountAsync()));
    }
    [SqlServerFact]
    public async Task Invalid_brand_does_not_persist()
    {
        using var response=await Client.PostAsJsonAsync("/api/brands",new CreateBrandRequest{Name="",Code="!"});Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);Assert.Equal(0,await ReadDatabaseAsync<BrandsDbContext,int>(db=>db.Brands.CountAsync()));
    }
    [SqlServerFact]
    public async Task Contributor_cannot_create_brand()
    {
        using var contributor=await Factory.CreateAuthenticatedClientAsync(RoleConstants.Contributor);using var response=await contributor.PostAsJsonAsync("/api/brands",new CreateBrandRequest{Name="Brand",Code="FORBIDDEN"});Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);
    }
}
