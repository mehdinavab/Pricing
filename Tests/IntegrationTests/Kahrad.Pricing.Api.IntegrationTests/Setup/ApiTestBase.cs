using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.Setup;

[Collection("ApiTests")]
public abstract class ApiTestBase(KahradPricingApiWebAppFactory factory) : IAsyncLifetime
{
    protected KahradPricingApiWebAppFactory Factory { get; } = factory;
    protected Kahrad.Pricing.Identity.Application.DTOs.Authentication.TokenResponseDto Authentication { get; private set; } = null!;
    protected HttpClient Client { get; private set; } = null!;
    protected static JsonSerializerOptions JsonOptions { get; } = new(JsonSerializerDefaults.Web)
    { Converters = { new JsonStringEnumConverter() } };

    public async Task InitializeAsync()
    {
        await Factory.ResetDbAsync();
        await Factory.PopulateAuthenticationAsync();
        Client = Factory.CreateClient(new() { AllowAutoRedirect = false });
        var tokens = await Factory.LoginAsync(Client,KahradPricingApiWebAppFactory.TestUsername,KahradPricingApiWebAppFactory.TestPassword);
        Authentication = tokens;
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",tokens.AccessToken);
    }
    public async Task DisposeAsync()
    {
        Client?.Dispose();
        await Factory.ResetDbAsync();
    }
    protected HttpClient AnonymousClient() => Factory.CreateClient(new() { AllowAutoRedirect = false });
    protected static async Task<T> ReadResultAsync<T>(HttpResponseMessage response)
    {
        var result = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        Assert.NotNull(result); return result;
    }
    protected async Task<TResult> ReadDatabaseAsync<TContext,TResult>(Func<TContext,Task<TResult>> query) where TContext:DbContext
    {
        await using var scope = Factory.Services.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        return await query(context);
    }
}
