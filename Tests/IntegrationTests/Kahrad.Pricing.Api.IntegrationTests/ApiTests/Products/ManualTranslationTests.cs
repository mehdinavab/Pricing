using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Kahrad.Pricing.Products.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Products;
public sealed class ManualTranslationTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Upsert_list_and_optimistic_concurrency_work_through_http()
    {
        var product=await Factory.CreateProductAsync();var path="/api/products/admin/products/translations";
        using var create=await Client.PostAsJsonAsync(path+"/upsert",new TranslationRequest(product.Id,"en","First name"));Assert.Equal(HttpStatusCode.NoContent,create.StatusCode);
        using var list=await Client.PostAsJsonAsync(path+"/list",new TranslationListRequest(product.Id));Assert.Equal(HttpStatusCode.OK,list.StatusCode);
        var translation=Assert.Single(await ReadResultAsync<List<TranslationDto>>(list),x=>x.LanguageCode=="en");
        using var update=await Client.PostAsJsonAsync(path+"/upsert",new TranslationRequest(product.Id,"en","Updated name",RowVersion:translation.RowVersion));Assert.Equal(HttpStatusCode.NoContent,update.StatusCode);
        using var stale=await Client.PostAsJsonAsync(path+"/upsert",new TranslationRequest(product.Id,"en","Stale name",RowVersion:translation.RowVersion));Assert.Equal(HttpStatusCode.Conflict,stale.StatusCode);
        Assert.Equal("Updated name",await ReadDatabaseAsync<ProductsDbContext,string>(db=>db.ProductTranslations.Where(x=>x.ProductId==product.Id&&db.Languages.Any(l=>l.Id==x.LanguageId&&l.Code=="en")).Select(x=>x.Name).SingleAsync()));
    }
    [SqlServerFact]
    public async Task Administrators_can_edit_inactive_language_but_public_resolution_ignores_it()
    {
        var product=await Factory.CreateProductAsync();using var language=await Client.PostAsJsonAsync("/api/products/admin/languages/upsert",new LanguageRequest("fr","French",false));Assert.Equal(HttpStatusCode.NoContent,language.StatusCode);
        using var translated=await Client.PostAsJsonAsync("/api/products/admin/products/translations/upsert",new TranslationRequest(product.Id,"fr","Produit"));Assert.Equal(HttpStatusCode.NoContent,translated.StatusCode);
        using var anonymous=AnonymousClient();anonymous.DefaultRequestHeaders.AcceptLanguage.ParseAdd("fr");
        using var response=await anonymous.PostAsJsonAsync("/api/products/details",new CatalogDetailsRequest(product.Id));var result=await ReadResultAsync<ProductDetailDto>(response);
        Assert.Equal("fa",result.Language);Assert.Equal(CatalogSetupExtensionMethods.PersianProductName,result.Name);
    }
    [SqlServerFact]
    public async Task Only_default_language_cannot_be_disabled()
    {
        using var list=await Client.PostAsJsonAsync("/api/products/admin/languages/list",new{});var fallback=Assert.Single(await ReadResultAsync<List<LanguageDto>>(list),x=>x.IsDefault);
        using var response=await Client.PostAsJsonAsync("/api/products/admin/languages/upsert",new LanguageRequest(fallback.Code,fallback.Name,false,false,fallback.RowVersion));
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        Assert.True(await ReadDatabaseAsync<ProductsDbContext,bool>(db=>db.Languages.AnyAsync(x=>x.IsActive&&x.IsDefault)));
    }
    [SqlServerTheory]
    [InlineData(TranslationTarget.Product)]
    [InlineData(TranslationTarget.Feature)]
    [InlineData(TranslationTarget.Specification)]
    [InlineData(TranslationTarget.ProductSpecification)]
    public async Task Missing_translation_report_is_language_specific(TranslationTarget target)
    {
        var product=await Factory.CreateProductAsync();var entityId=product.Id;
        if(target is TranslationTarget.Feature or TranslationTarget.Specification or TranslationTarget.ProductSpecification)
        {
            var path=target==TranslationTarget.Feature?"features":"specifications";
            using var definition=await Client.PostAsJsonAsync($"/api/products/admin/{path}/create",new DefinitionRequest("DEFINITION","Definition"));
            Assert.Equal(HttpStatusCode.OK,definition.StatusCode);entityId=await ReadResultAsync<long>(definition);
            if(target==TranslationTarget.ProductSpecification)
            {
                using var assignment=await Client.PostAsJsonAsync("/api/products/admin/product-specifications/upsert",new SpecificationAssignmentRequest(product.Id,entityId,TextValue:"Wood"));
                Assert.Equal(HttpStatusCode.OK,assignment.StatusCode);entityId=await ReadResultAsync<long>(assignment);
            }
        }
        using var response=await Client.PostAsJsonAsync("/api/products/admin/translations/missing",new MissingTranslationsRequest(target,"ar"));
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);Assert.Equal(entityId,Assert.Single(await ReadResultAsync<List<MissingTranslationDto>>(response)).EntityId);
    }
    [SqlServerFact]
    public async Task Anonymous_translation_write_is_rejected_without_persistence()
    {
        var product=await Factory.CreateProductAsync();using var anonymous=AnonymousClient();
        using var response=await anonymous.PostAsJsonAsync("/api/products/admin/products/translations/upsert",new TranslationRequest(product.Id,"en","Unauthorized"));Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
        Assert.Equal(1,await ReadDatabaseAsync<ProductsDbContext,int>(db=>db.ProductTranslations.CountAsync()));
    }
}
