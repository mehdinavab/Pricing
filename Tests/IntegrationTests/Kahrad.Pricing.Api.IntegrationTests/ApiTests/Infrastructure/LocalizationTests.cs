using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Infrastructure;

public sealed class LocalizationTests(KahradPricingApiWebAppFactory factory) : ApiTestBase(factory)
{
    [SqlServerTheory]
    [InlineData("en-US", "Product name is required.")]
    [InlineData("fa-IR", "نام محصول لازم است.")]
    [InlineData("ar-SA", "اسم المنتج مطلوب.")]
    public async Task Validation_uses_request_language(string language, string expected)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/products")
        { Content = JsonContent.Create(new CreateProductRequest { Name = "", Code = "X", GroupId = 1 }) };
        request.Headers.Add("Accept-Language", language);
        using var response = await Client.SendAsync(request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(language, response.Content.Headers.ContentLanguage.Single());
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problem);
        Assert.Contains(expected, problem.Errors["Name"]);
        Assert.Equal(language switch
        {
            "fa-IR" => "درخواست نامعتبر است.",
            "ar-SA" => "الطلب غير صالح.",
            _ => "The request is invalid."
        }, problem.Title);
    }

    [SqlServerTheory]
    [InlineData("fa-IR", "گروه محصول یافت نشد.")]
    [InlineData("ar-SA", "لم يتم العثور على مجموعة المنتجات.")]
    public async Task Exception_uses_request_language(string language, string expected)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/products")
        { Content = JsonContent.Create(new CreateProductRequest { Name = "Valid", Code = "X", GroupId = long.MaxValue }) };
        request.Headers.Add("Accept-Language", language);
        using var response = await Client.SendAsync(request);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal(expected, problem?.Title);
        Assert.Contains(long.MaxValue.ToString(), problem?.Detail);
        Assert.DoesNotContain("was not found", problem?.Detail);
    }

    [SqlServerFact]
    public async Task Region_selects_language_when_accept_language_is_absent()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/products")
        { Content = JsonContent.Create(new CreateProductRequest { Name = "", Code = "X", GroupId = 1 }) };
        request.Headers.Add("X-Region", "IR");
        using var response = await Client.SendAsync(request);
        Assert.Equal("fa-IR", response.Content.Headers.ContentLanguage.Single());
        var problem = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.Contains("نام محصول لازم است.", problem!.Errors["Name"]);
    }

    [SqlServerFact]
    public async Task Accept_language_takes_priority_over_region()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, "/api/products")
        { Content = JsonContent.Create(new CreateProductRequest { Name = "", Code = "X", GroupId = 1 }) };
        request.Headers.Add("Accept-Language", "ar-SA");
        request.Headers.Add("X-Region", "IR");
        using var response = await Client.SendAsync(request);
        Assert.Equal("ar-SA", response.Content.Headers.ContentLanguage.Single());
    }
}
