using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Products;
public sealed class CatalogLocalizationTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    private const string ArabicProduct="\u0645\u0646\u062a\u062c \u0639\u0631\u0628\u064a";
    [SqlServerFact]
    public async Task Region_header_selects_catalog_translation()
    {
        var product=await Factory.CreateProductAsync();
        using var translated=await Client.PostAsJsonAsync("/api/products/admin/products/translations/upsert",new TranslationRequest(product.Id,"en","English product"));
        Assert.Equal(HttpStatusCode.NoContent,translated.StatusCode);
        using var anonymous=AnonymousClient();
        anonymous.DefaultRequestHeaders.Add("X-Region","US");
        using var response=await anonymous.PostAsJsonAsync("/api/products/details",new CatalogDetailsRequest(product.Id));
        Assert.Equal("English product",(await ReadResultAsync<ProductDetailDto>(response)).Name);
        Assert.Equal("en",Assert.Single(response.Content.Headers.ContentLanguage));
    }
    [SqlServerTheory]
    [InlineData("fa",CatalogSetupExtensionMethods.PersianProductName,"fa")]
    [InlineData("ar",ArabicProduct,"ar")]
    [InlineData("en","English product","en")]
    [InlineData("en-US","English product","en")]
    [InlineData("ar;q=0,en;q=0.9,fa;q=0.5","English product","en")]
    public async Task Public_details_resolves_real_translations_and_quality_preferences(string header,string expected,string language)
    {
        var product=await Factory.CreateProductAsync();
        using var ar=await Client.PostAsJsonAsync("/api/products/admin/products/translations/upsert",new TranslationRequest(product.Id,"ar",ArabicProduct));Assert.Equal(HttpStatusCode.NoContent,ar.StatusCode);
        using var en=await Client.PostAsJsonAsync("/api/products/admin/products/translations/upsert",new TranslationRequest(product.Id,"en","English product"));Assert.Equal(HttpStatusCode.NoContent,en.StatusCode);
        using var anonymous=AnonymousClient();anonymous.DefaultRequestHeaders.AcceptLanguage.ParseAdd(header);
        using var response=await anonymous.PostAsJsonAsync("/api/products/details",new CatalogDetailsRequest(product.Id));
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);var result=await ReadResultAsync<ProductDetailDto>(response);
        Assert.Equal(expected,result.Name);Assert.Equal(language,result.Language);Assert.Equal(language,Assert.Single(response.Content.Headers.ContentLanguage));
        Assert.Contains("Accept-Language",response.Headers.Vary);Assert.Equal(100m,result.BasePrice);
    }
    [SqlServerFact]
    public async Task Missing_requested_translation_falls_back_to_actual_persian_without_fabrication()
    {
        var product=await Factory.CreateProductAsync();using var anonymous=AnonymousClient();anonymous.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ar");
        using var response=await anonymous.PostAsJsonAsync("/api/products/details",new CatalogDetailsRequest(product.Id));
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);Assert.Equal(CatalogSetupExtensionMethods.PersianProductName,(await ReadResultAsync<ProductDetailDto>(response)).Name);
    }
    [SqlServerFact]
    public async Task Localized_search_filters_counts_and_paginates_distinct_products()
    {
        var first=await Factory.CreateProductAsync();await Factory.CreateProductAsync();
        using var translated=await Client.PostAsJsonAsync("/api/products/admin/products/translations/upsert",new TranslationRequest(first.Id,"en","Searchable English name"));Assert.Equal(HttpStatusCode.NoContent,translated.StatusCode);
        using var anonymous=AnonymousClient();anonymous.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en");
        using var filtered=await anonymous.PostAsJsonAsync("/api/products/search",new CatalogSearchRequest{Name="Searchable",PageSize=1});
        Assert.Equal(HttpStatusCode.OK,filtered.StatusCode);var match=await ReadResultAsync<BaseResponsePaginated<ProductDetailDto>>(filtered);Assert.Equal(1,match.TotalCount);Assert.Equal(first.Id,Assert.Single(match.Data).Id);
        using var page1=await anonymous.PostAsJsonAsync("/api/products/search",new CatalogSearchRequest{Page=1,PageSize=1});
        using var page2=await anonymous.PostAsJsonAsync("/api/products/search",new CatalogSearchRequest{Page=2,PageSize=1});
        var firstPage=await ReadResultAsync<BaseResponsePaginated<ProductDetailDto>>(page1);var secondPage=await ReadResultAsync<BaseResponsePaginated<ProductDetailDto>>(page2);
        Assert.Equal(2,firstPage.TotalCount);Assert.Equal(2,secondPage.TotalCount);Assert.NotEqual(Assert.Single(firstPage.Data).Id,Assert.Single(secondPage.Data).Id);
    }
    [SqlServerFact]
    public async Task Invalid_pagination_is_rejected()
    {
        using var anonymous=AnonymousClient();using var response=await anonymous.PostAsJsonAsync("/api/products/search",new CatalogSearchRequest{Page=0,PageSize=101});
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
    }
}
