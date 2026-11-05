using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Kahrad.Pricing.Products.Infrastructure.Persistence;
using Kahrad.Pricing.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Products;
public sealed class CreateProductTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Create_persists_trimmed_unicode_default_translation_and_real_audit()
    {
        var group=await Factory.CreateProductGroupAsync();
        using var response=await Client.PostAsJsonAsync("/api/products",new CreateProductRequest{Name="  "+CatalogSetupExtensionMethods.PersianProductName+"  ",Code="sku-01",GroupId=group.Id,BasePrice=125.5m});
        Assert.Equal(HttpStatusCode.Created,response.StatusCode);var id=await ReadResultAsync<long>(response);
        var product=await ReadDatabaseAsync<ProductsDbContext,Kahrad.Pricing.Products.Domain.Entities.Product>(db=>db.Products.AsNoTracking().SingleAsync(x=>x.Id==id));
        Assert.Equal("SKU-01",product.Code);Assert.Equal(CatalogSetupExtensionMethods.PersianProductName,product.Name);Assert.Equal(125.5m,product.BasePrice);
        var translation=await ReadDatabaseAsync<ProductsDbContext,string>(db=>db.ProductTranslations.Where(x=>x.ProductId==id).Select(x=>x.Name).SingleAsync());
        Assert.Equal(CatalogSetupExtensionMethods.PersianProductName,translation);
        Assert.True(await ReadDatabaseAsync<IdentityDbContext,bool>(db=>db.AuditLogs.AnyAsync(x=>x.Action=="ProductCreated"&&x.CreatedById!=null)));
    }
    [SqlServerFact]
    public async Task Duplicate_code_does_not_duplicate_product_or_translation()
    {
        var group=await Factory.CreateProductGroupAsync();var request=new CreateProductRequest{Name="Product",Code="SAME",GroupId=group.Id};
        using var first=await Client.PostAsJsonAsync("/api/products",request);Assert.Equal(HttpStatusCode.Created,first.StatusCode);
        using var duplicate=await Client.PostAsJsonAsync("/api/products",request);Assert.Equal(HttpStatusCode.BadRequest,duplicate.StatusCode);
        Assert.Equal(1,await ReadDatabaseAsync<ProductsDbContext,int>(db=>db.Products.CountAsync()));
        Assert.Equal(1,await ReadDatabaseAsync<ProductsDbContext,int>(db=>db.ProductTranslations.CountAsync()));
    }
    [SqlServerFact]
    public async Task Invalid_input_returns_400_without_persistence()
    {
        using var response=await Client.PostAsJsonAsync("/api/products",new CreateProductRequest{Name=" ",Code="BAD CODE",GroupId=0,BasePrice=-1});
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);Assert.Equal(0,await ReadDatabaseAsync<ProductsDbContext,int>(db=>db.Products.CountAsync()));
    }
    [SqlServerFact]
    public async Task Missing_group_returns_404_without_persistence()
    {
        using var response=await Client.PostAsJsonAsync("/api/products",new CreateProductRequest{Name="Product",Code="NO-GROUP",GroupId=long.MaxValue});
        Assert.Equal(HttpStatusCode.NotFound,response.StatusCode);Assert.Equal(0,await ReadDatabaseAsync<ProductsDbContext,int>(db=>db.Products.CountAsync()));
    }
}
