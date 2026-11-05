using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Users;
public sealed class SuperAdminProtectionTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    [SqlServerTheory]
    [InlineData("reset-password","POST")][InlineData("status","PATCH")][InlineData("profile","PUT")]
    [InlineData("remove-role","DELETE")][InlineData("assign-role","POST")]
    public async Task Admin_cannot_mutate_a_real_superadmin_account(string action,string method)
    {
        var user=await ReadDatabaseAsync<IdentityDbContext,Kahrad.Pricing.Identity.Domain.Entities.ApplicationUser>(db=>db.Users.AsNoTracking().SingleAsync(x=>x.UserName==KahradPricingApiWebAppFactory.TestUsername));
        var roleId=await ReadDatabaseAsync<IdentityDbContext,Guid>(db=>db.Roles.Where(x=>x.Name==RoleConstants.SuperAdmin).Select(x=>x.Id).SingleAsync());
        using var admin=await Factory.CreateAuthenticatedClientAsync(RoleConstants.Admin);
        var path=action switch {"profile"=>$"/api/users/{user.Id}","remove-role" or "assign-role"=>$"/api/users/{user.Id}/roles/{roleId}",_=>$"/api/users/{user.Id}/{action}"};
        using var request=new HttpRequestMessage(new HttpMethod(method),path);
        if(action=="reset-password")request.Content=JsonContent.Create(new{NewPassword="New-valid-password123!"});
        if(action=="status")request.Content=JsonContent.Create(new{IsActive=false});
        if(action=="profile")request.Content=JsonContent.Create(new{FirstName="Changed",LastName="User",Email="changed@example.test",PhoneNumber="09121234567"});
        using var response=await admin.SendAsync(request);Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);
        var after=await ReadDatabaseAsync<IdentityDbContext,Kahrad.Pricing.Identity.Domain.Entities.ApplicationUser>(db=>db.Users.AsNoTracking().SingleAsync(x=>x.Id==user.Id));
        Assert.True(after.PasswordHash==user.PasswordHash,"Protected password changed.");Assert.True(after.IsActive);Assert.Equal(user.FirstName,after.FirstName);
        Assert.True(await ReadDatabaseAsync<IdentityDbContext,bool>(db=>db.UserRoles.AnyAsync(x=>x.UserId==user.Id&&x.RoleId==roleId)));
    }
    [SqlServerFact]
    public async Task Admin_can_reset_an_ordinary_users_password()
    {
        var user=await Factory.CreateUserAsync();using var admin=await Factory.CreateAuthenticatedClientAsync(RoleConstants.Admin);
        using var response=await admin.PostAsJsonAsync($"/api/users/{user.Id}/reset-password",new{NewPassword="Replacement-password123!"});
        Assert.Equal(HttpStatusCode.NoContent,response.StatusCode);
        using var anonymous=AnonymousClient();var tokens=await Factory.LoginAsync(anonymous,user.UserName!,"Replacement-password123!");
        Assert.False(string.IsNullOrWhiteSpace(tokens.AccessToken));
    }
}
