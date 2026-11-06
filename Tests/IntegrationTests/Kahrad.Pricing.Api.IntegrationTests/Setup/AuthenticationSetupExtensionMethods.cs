using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Identity.Domain.Entities;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Kahrad.Pricing.Identity.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.Setup;

public static class AuthenticationSetupExtensionMethods
{
    public static async Task PopulateAuthenticationAsync(this KahradPricingApiWebAppFactory factory)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IdentitySeeder>().SeedRolesAsync();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = new ApplicationUser(KahradPricingApiWebAppFactory.TestUsername,"Integration","SuperAdmin","superadmin@example.test");
        user.PhoneNumber = "09121234567";
        EnsureSucceeded(await users.CreateAsync(user,KahradPricingApiWebAppFactory.TestPassword));
        EnsureSucceeded(await users.AddToRoleAsync(user,RoleConstants.SuperAdmin));
    }
    public static async Task<ApplicationUser> CreateUserAsync(this KahradPricingApiWebAppFactory factory,string role=RoleConstants.Contributor,bool active=true)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = new ApplicationUser("user-"+Guid.NewGuid().ToString("N"),"Integration","User");
        user.SetActiveStatus(active);
        EnsureSucceeded(await users.CreateAsync(user,KahradPricingApiWebAppFactory.TestPassword));
        EnsureSucceeded(await users.AddToRoleAsync(user,role)); return user;
    }
    public static async Task<TokenResponseDto> LoginAsync(this KahradPricingApiWebAppFactory factory,HttpClient client,string username,string password)
    {
        using var response = await client.PostAsJsonAsync("/api/identity/auth/login",new PasswordLoginRequest { Username=username,Password=password });
        Assert.True(response.StatusCode==HttpStatusCode.OK,$"Test login failed with HTTP {(int)response.StatusCode}.");
        var tokens = await response.Content.ReadFromJsonAsync<TokenResponseDto>();
        Assert.NotNull(tokens);Assert.False(string.IsNullOrWhiteSpace(tokens.AccessToken));return tokens;
    }
    public static async Task<HttpClient> CreateAuthenticatedClientAsync(this KahradPricingApiWebAppFactory factory,string role)
    {
        var user = await factory.CreateUserAsync(role);
        var client = factory.CreateClient(new(){AllowAutoRedirect=false});
        try
        {
            var tokens = await factory.LoginAsync(client,user.UserName!,KahradPricingApiWebAppFactory.TestPassword);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",tokens.AccessToken);return client;
        }
        catch { client.Dispose();throw; }
    }
    private static void EnsureSucceeded(IdentityResult result) => Assert.True(result.Succeeded,string.Join(", ",result.Errors.Select(x=>x.Code)));
}
