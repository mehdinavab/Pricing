using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Kahrad.Pricing.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Authentication;
public sealed class RefreshTokenTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Refresh_rotates_and_links_the_token_family_atomically()
    {
        using var response=await Client.PostAsJsonAsync("/api/identity/auth/refresh",new RefreshTokenRequest{RefreshToken=Authentication.RefreshToken});
        Assert.Equal(HttpStatusCode.OK,response.StatusCode);var replacement=await ReadResultAsync<TokenResponseDto>(response);
        Assert.False(replacement.RefreshToken==Authentication.RefreshToken);
        var oldHash=Hash(Authentication.RefreshToken);var newHash=Hash(replacement.RefreshToken);
        var tokens=await ReadDatabaseAsync<IdentityDbContext,List<Kahrad.Pricing.Identity.Domain.Entities.RefreshToken>>(db=>db.RefreshTokens.AsNoTracking().ToListAsync());
        var previous=Assert.Single(tokens,x=>x.TokenHash==oldHash);var current=Assert.Single(tokens,x=>x.TokenHash==newHash);
        Assert.True(previous.IsRevoked);Assert.Equal(current.Id,previous.ReplacedByTokenId);Assert.Equal(previous.FamilyId,current.FamilyId);Assert.False(current.IsRevoked);
    }
    [SqlServerFact]
    public async Task Reusing_an_old_refresh_token_revokes_its_replacement()
    {
        using var first=await Client.PostAsJsonAsync("/api/identity/auth/refresh",new RefreshTokenRequest{RefreshToken=Authentication.RefreshToken});
        Assert.Equal(HttpStatusCode.OK,first.StatusCode);var replacement=await ReadResultAsync<TokenResponseDto>(first);
        using var replay=await Client.PostAsJsonAsync("/api/identity/auth/refresh",new RefreshTokenRequest{RefreshToken=Authentication.RefreshToken});
        Assert.Equal(HttpStatusCode.Unauthorized,replay.StatusCode);
        using var later=await Client.PostAsJsonAsync("/api/identity/auth/refresh",new RefreshTokenRequest{RefreshToken=replacement.RefreshToken});
        Assert.Equal(HttpStatusCode.Unauthorized,later.StatusCode);
    }
    [SqlServerFact]
    public async Task Logout_revokes_the_supplied_session()
    {
        using var logout=await Client.PostAsJsonAsync("/api/identity/auth/logout",new RefreshTokenRequest{RefreshToken=Authentication.RefreshToken});
        Assert.Equal(HttpStatusCode.NoContent,logout.StatusCode);
        using var refresh=await Client.PostAsJsonAsync("/api/identity/auth/refresh",new RefreshTokenRequest{RefreshToken=Authentication.RefreshToken});
        Assert.Equal(HttpStatusCode.Unauthorized,refresh.StatusCode);
    }
    [SqlServerFact]
    public async Task Logout_all_revokes_every_current_users_refresh_session()
    {
        using var other=AnonymousClient();await Factory.LoginAsync(other,KahradPricingApiWebAppFactory.TestUsername,KahradPricingApiWebAppFactory.TestPassword);
        using var response=await Client.PostAsJsonAsync("/api/identity/auth/logout-all",new{});Assert.Equal(HttpStatusCode.NoContent,response.StatusCode);
        Assert.False(await ReadDatabaseAsync<IdentityDbContext,bool>(db=>db.RefreshTokens.AnyAsync(x=>x.RevokedAtUtc==null)));
    }
    [SqlServerFact]
    public async Task Unknown_refresh_token_is_rejected()
    {
        using var response=await Client.PostAsJsonAsync("/api/identity/auth/refresh",new RefreshTokenRequest{RefreshToken="not-a-session"});
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
    private static string Hash(string token)=>Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
}
