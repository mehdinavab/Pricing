using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Kahrad.Pricing.Products.Domain.Enums;
using Kahrad.Pricing.Products.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Products;
public sealed class SharedCatalogDefinitionTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task One_shared_label_localizes_multiple_products_and_text_values_fall_back_independently()
    {
        var first=await Factory.CreateProductAsync();var second=await Factory.CreateProductAsync();
        using var featureResponse=await Client.PostAsJsonAsync("/api/products/admin/features/create",new DefinitionRequest("WATERPROOF","\u0636\u062f \u0622\u0628"));Assert.Equal(HttpStatusCode.OK,featureResponse.StatusCode);var featureId=await ReadResultAsync<long>(featureResponse);
        using var specificationResponse=await Client.PostAsJsonAsync("/api/products/admin/specifications/create",new DefinitionRequest("MATERIAL","\u062c\u0646\u0633"));Assert.Equal(HttpStatusCode.OK,specificationResponse.StatusCode);var specificationId=await ReadResultAsync<long>(specificationResponse);
        using var featureLabel=await Client.PostAsJsonAsync("/api/products/admin/features/translations/upsert",new TranslationRequest(featureId,"en","Waterproof"));Assert.Equal(HttpStatusCode.NoContent,featureLabel.StatusCode);
        using var specLabel=await Client.PostAsJsonAsync("/api/products/admin/specifications/translations/upsert",new TranslationRequest(specificationId,"en","Material"));Assert.Equal(HttpStatusCode.NoContent,specLabel.StatusCode);
        foreach(var product in new[]{first,second})
        {
            using var assignment=await Client.PostAsJsonAsync($"/api/products/{product.Id}/features",new{FeatureId=featureId,Value="yes",HasPriceEffect=false});Assert.Equal(HttpStatusCode.Created,assignment.StatusCode);
            using var spec=await Client.PostAsJsonAsync("/api/products/admin/product-specifications/upsert",new SpecificationAssignmentRequest(product.Id,specificationId,TextValue:"\u0686\u0648\u0628"));Assert.Equal(HttpStatusCode.OK,spec.StatusCode);
            if(product.Id==first.Id)
            {
                var assignmentId=await ReadResultAsync<long>(spec);
                using var text=await Client.PostAsJsonAsync("/api/products/admin/product-specifications/translations/upsert",new TranslationRequest(assignmentId,"en","Wood"));Assert.Equal(HttpStatusCode.NoContent,text.StatusCode);
            }
        }
        using var anonymous=AnonymousClient();anonymous.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en");
        foreach(var product in new[]{first,second})
        {
            using var response=await anonymous.PostAsJsonAsync("/api/products/details",new CatalogDetailsRequest(product.Id));var details=await ReadResultAsync<ProductDetailDto>(response);
            Assert.Equal("Waterproof",Assert.Single(details.Features).Name);var spec=Assert.Single(details.Specifications);Assert.Equal("Material",spec.Name);
            Assert.Equal(product.Id==first.Id?"Wood":"\u0686\u0648\u0628",spec.TextValue);
        }
        Assert.Equal(1,await ReadDatabaseAsync<ProductsDbContext,int>(db=>db.Features.CountAsync()));
        Assert.Equal(1,await ReadDatabaseAsync<ProductsDbContext,int>(db=>db.Specifications.CountAsync()));
    }
    [SqlServerFact]
    public async Task Numeric_specification_stays_decimal_and_cannot_receive_a_text_translation()
    {
        var product=await Factory.CreateProductAsync();using var definition=await Client.PostAsJsonAsync("/api/products/admin/specifications/create",new DefinitionRequest("WEIGHT","Weight",SpecificationValueType.Number,"kg"));var definitionId=await ReadResultAsync<long>(definition);
        using var assignment=await Client.PostAsJsonAsync("/api/products/admin/product-specifications/upsert",new SpecificationAssignmentRequest(product.Id,definitionId,NumberValue:12.5m));Assert.Equal(HttpStatusCode.OK,assignment.StatusCode);var id=await ReadResultAsync<long>(assignment);
        using var text=await Client.PostAsJsonAsync("/api/products/admin/product-specifications/translations/upsert",new TranslationRequest(id,"en","Twelve"));Assert.Equal(HttpStatusCode.BadRequest,text.StatusCode);
        var value=await ReadDatabaseAsync<ProductsDbContext,Kahrad.Pricing.Products.Domain.Entities.ProductSpecification>(db=>db.ProductSpecifications.AsNoTracking().SingleAsync(x=>x.Id==id));Assert.Equal(12.5m,value.NumberValue);Assert.Null(value.TextValue);
    }
}
