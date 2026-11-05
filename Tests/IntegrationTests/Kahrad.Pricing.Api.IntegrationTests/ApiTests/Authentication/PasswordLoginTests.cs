using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Identity.Application.DTOs.Authentication;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Authentication;
public sealed class PasswordLoginTests(KahradPricingApiWebAppFactory factory) : ApiTestBase(factory)
{
    [SqlServerFact(DisplayName="Login => seeded SuperAdmin => real JWT and hashed refresh token")]
    public async Task Login_seeded_superadmin_returns_tokens_and_persists_only_refresh_hash()
    {
        using var client=AnonymousClient();var result=await Factory.LoginAsync(client,KahradPricingApiWebAppFactory.TestUsername,KahradPricingApiWebAppFactory.TestPassword);
        Assert.True(result.AccessTokenExpiresAtUtc>DateTime.UtcNow);Assert.True(result.RefreshTokenExpiresAtUtc>result.AccessTokenExpiresAtUtc);
        var jwt=new JwtSecurityTokenHandler().ReadJwtToken(result.AccessToken);
        Assert.Contains(jwt.Claims,x=>(x.Type==ClaimTypes.Role||x.Type=="role")&&x.Value==RoleConstants.SuperAdmin);
        var tokens=await ReadDatabaseAsync<IdentityDbContext,List<string>>(db=>db.RefreshTokens.Select(x=>x.TokenHash).ToListAsync());
        Assert.All(tokens,hash=>{Assert.Equal(64,hash.Length);Assert.False(hash==result.RefreshToken);});
    }
    [SqlServerTheory(DisplayName="Login => unknown username or wrong password => unauthorized")]
    [InlineData(true)][InlineData(false)]
    public async Task Invalid_credentials_are_rejected(bool unknownUsername)
    {
        using var client=AnonymousClient();using var response=await client.PostAsJsonAsync("/api/identity/auth/login",new PasswordLoginRequest
        {Username=unknownUsername?"missing.user":KahradPricingApiWebAppFactory.TestUsername,Password=unknownUsername?KahradPricingApiWebAppFactory.TestPassword:"Incorrect-password123!"});
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
    [SqlServerFact]
    public async Task Inactive_user_cannot_login()
    {
        var user=await Factory.CreateUserAsync(active:false);using var client=AnonymousClient();
        using var response=await client.PostAsJsonAsync("/api/identity/auth/login",new PasswordLoginRequest{Username=user.UserName!,Password=KahradPricingApiWebAppFactory.TestPassword});
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
    [SqlServerFact]
    public async Task Missing_password_returns_validation_error_without_creating_token()
    {
        var before=await ReadDatabaseAsync<IdentityDbContext,int>(db=>db.RefreshTokens.CountAsync());using var client=AnonymousClient();
        using var response=await client.PostAsJsonAsync("/api/identity/auth/login",new PasswordLoginRequest{Username=KahradPricingApiWebAppFactory.TestUsername,Password=""});
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        Assert.Equal(before,await ReadDatabaseAsync<IdentityDbContext,int>(db=>db.RefreshTokens.CountAsync()));
    }
}
