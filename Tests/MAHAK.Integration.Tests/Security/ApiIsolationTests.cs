using System.Net;
using System.Net.Http.Json;
using MAHAK.Integration.Tests.TestSupport;
using Xunit;

namespace MAHAK.Integration.Tests.Security;

public sealed class ApiIsolationTests
{
    [Fact]
    public async Task Authenticated_keys_route_to_their_own_configured_catalogs()
    {
        await using var factory = new MahakApiFactory();
        using var client = factory.CreateClient();

        var kahrad = Request(MahakTestData.KahradKey, """{"scope":"ReturnedItemsOnly","pageSize":10}""");
        var rodor = Request(MahakTestData.RodorKey, """{"scope":"ReturnedItemsOnly","pageSize":10}""");
        Assert.Equal(HttpStatusCode.OK, (await client.SendAsync(kahrad)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.SendAsync(rodor)).StatusCode);

        Assert.Collection(
            factory.Items.Calls,
            call => Assert.Equal("Catalog_Kahrad", call.Database),
            call => Assert.Equal("Catalog_Rodor", call.Database));
    }

    [Fact]
    public async Task Caller_database_or_brand_fields_are_rejected_before_repository_access()
    {
        await using var factory = new MahakApiFactory();
        using var client = factory.CreateClient();
        using var request = Request(
            MahakTestData.KahradKey,
            """{"scope":"ReturnedItemsOnly","pageSize":10,"brandId":"Rodor","database":"Catalog_Rodor"}""");

        var response = await client.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(factory.Items.Calls);
    }

    [Theory]
    [InlineData("wrong")]
    [InlineData(MahakTestData.DisabledKey)]
    public async Task Wrong_or_disabled_key_is_rejected_without_database_access(string key)
    {
        await using var factory = new MahakApiFactory();
        using var client = factory.CreateClient();
        using var request = Request(key, """{"scope":"ReturnedItemsOnly","pageSize":10}""");

        var response = await client.SendAsync(request);
        var body = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Empty(factory.Items.Calls);
        Assert.DoesNotContain(MahakTestData.KahradKey, body);
        Assert.DoesNotContain("test-password", body);
    }

    [Fact]
    public async Task OpenApi_does_not_expose_configured_secrets()
    {
        await using var factory = new MahakApiFactory();
        using var client = factory.CreateClient();

        var document = await client.GetStringAsync("/openapi/v1.json");

        Assert.DoesNotContain(MahakTestData.KahradKey, document);
        Assert.DoesNotContain(MahakTestData.RodorKey, document);
        Assert.DoesNotContain("test-password", document);
        Assert.DoesNotContain("Catalog_Kahrad", document);
        Assert.Contains("X-API-Key", document);
    }

    private static HttpRequestMessage Request(string apiKey, string json)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/items/sync")
        {
            Content = JsonContent.Create(
                System.Text.Json.JsonDocument.Parse(json).RootElement)
        };
        request.Headers.Add("X-API-Key", apiKey);
        return request;
    }
}
