using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;
using Kahrad.Pricing.Guarantees.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Guarantees;
public sealed class VendorEndpointTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Create_update_and_delete_vendor_preserve_soft_delete_convention()
    {
        using var created=await Client.PostAsJsonAsync("/api/guarantee-vendors",new CreateVendorRequest{Name="  Vendor One  ",Code="vendor-one"});Assert.Equal(HttpStatusCode.Created,created.StatusCode);var id=await ReadResultAsync<long>(created);
        var vendor=await ReadDatabaseAsync<GuaranteesDbContext,Kahrad.Pricing.Guarantees.Domain.Entities.Vendor>(db=>db.Vendors.AsNoTracking().SingleAsync(x=>x.Id==id));Assert.Equal("Vendor One",vendor.Name);Assert.Equal("VENDOR-ONE",vendor.Code);
        using var update=await Client.PutAsJsonAsync($"/api/guarantee-vendors/{id}",new UpdateVendorRequest{Name="Updated vendor",Code="VENDOR-ONE"});Assert.Equal(HttpStatusCode.NoContent,update.StatusCode);
        using var details=await Client.GetAsync($"/api/guarantee-vendors/{id}");Assert.Equal("Updated vendor",(await ReadResultAsync<VendorDto>(details)).Name);
        using var delete=await Client.DeleteAsync($"/api/guarantee-vendors/{id}");Assert.Equal(HttpStatusCode.NoContent,delete.StatusCode);
        Assert.True(await ReadDatabaseAsync<GuaranteesDbContext,bool>(db=>db.Vendors.IgnoreQueryFilters().AnyAsync(x=>x.Id==id&&x.IsDeleted)));
    }
    [SqlServerFact]
    public async Task Duplicate_vendor_code_is_rejected_without_duplicate_record()
    {
        var request=new CreateVendorRequest{Name="Vendor",Code="DUPLICATE"};using var first=await Client.PostAsJsonAsync("/api/guarantee-vendors",request);Assert.Equal(HttpStatusCode.Created,first.StatusCode);
        using var second=await Client.PostAsJsonAsync("/api/guarantee-vendors",request);Assert.Equal(HttpStatusCode.BadRequest,second.StatusCode);Assert.Equal(1,await ReadDatabaseAsync<GuaranteesDbContext,int>(db=>db.Vendors.CountAsync()));
    }
    [SqlServerFact]
    public async Task Invalid_vendor_input_returns_400()
    {
        using var response=await Client.PostAsJsonAsync("/api/guarantee-vendors",new CreateVendorRequest{Name="",Code="!"});Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);Assert.Equal(0,await ReadDatabaseAsync<GuaranteesDbContext,int>(db=>db.Vendors.CountAsync()));
    }
    [SqlServerFact]
    public async Task Missing_vendor_returns_404()
    {
        using var response=await Client.GetAsync($"/api/guarantee-vendors/{long.MaxValue}");Assert.Equal(HttpStatusCode.NotFound,response.StatusCode);
    }
}
