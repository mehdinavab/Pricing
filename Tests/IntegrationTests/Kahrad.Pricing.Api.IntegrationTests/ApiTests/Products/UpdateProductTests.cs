using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Kahrad.Pricing.Products.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Products;
public sealed class UpdateProductTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Update_synchronizes_legacy_and_default_names_without_changing_pricing_factors()
    {
        var product=await Factory.CreateProductAsync();const string updated="\u0646\u0627\u0645 \u0648\u06cc\u0631\u0627\u06cc\u0634 \u0634\u062f\u0647";
        using var response=await Client.PutAsJsonAsync($"/api/products/{product.Id}",new UpdateProductRequest{Name=updated,Code=product.Code,GroupId=product.GroupId,BasePrice=250});
        Assert.Equal(HttpStatusCode.NoContent,response.StatusCode);
        var current=await ReadDatabaseAsync<ProductsDbContext,Kahrad.Pricing.Products.Domain.Entities.Product>(db=>db.Products.AsNoTracking().SingleAsync(x=>x.Id==product.Id));
        Assert.Equal(updated,current.Name);Assert.Equal(250m,current.BasePrice);Assert.Equal(product.BeautyFactorPercentage,current.BeautyFactorPercentage);
        Assert.Equal(updated,await ReadDatabaseAsync<ProductsDbContext,string>(db=>db.ProductTranslations.Where(x=>x.ProductId==product.Id).Select(x=>x.Name).SingleAsync()));
    }
    [SqlServerFact]
    public async Task Delete_soft_deletes_and_hides_public_product_while_preserving_translation()
    {
        var product=await Factory.CreateProductAsync();using var response=await Client.DeleteAsync($"/api/products/{product.Id}");
        Assert.Equal(HttpStatusCode.NoContent,response.StatusCode);
        Assert.True(await ReadDatabaseAsync<ProductsDbContext,bool>(db=>db.Products.IgnoreQueryFilters().AnyAsync(x=>x.Id==product.Id&&x.IsDeleted)));
        Assert.True(await ReadDatabaseAsync<ProductsDbContext,bool>(db=>db.ProductTranslations.AnyAsync(x=>x.ProductId==product.Id)));
        using var anonymous=AnonymousClient();using var details=await anonymous.PostAsJsonAsync("/api/products/details",new{ProductId=product.Id});Assert.Equal(HttpStatusCode.NotFound,details.StatusCode);
    }
    [SqlServerFact]
    public async Task Updating_missing_product_returns_404()
    {
        var group=await Factory.CreateProductGroupAsync();using var response=await Client.PutAsJsonAsync($"/api/products/{long.MaxValue}",new UpdateProductRequest{Name="Missing",Code="MISSING",GroupId=group.Id});
        Assert.Equal(HttpStatusCode.NotFound,response.StatusCode);
    }
}
