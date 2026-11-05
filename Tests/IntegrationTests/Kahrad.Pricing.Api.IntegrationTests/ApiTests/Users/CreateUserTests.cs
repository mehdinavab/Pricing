using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Identity.Application.DTOs.Users;
using Kahrad.Pricing.Identity.Domain.Constants;
using Kahrad.Pricing.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Users;
public sealed class CreateUserTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Create_user_persists_profile_role_hash_and_audit()
    {
        var roleId=await ReadDatabaseAsync<IdentityDbContext,Guid>(db=>db.Roles.Where(x=>x.Name==RoleConstants.Contributor).Select(x=>x.Id).SingleAsync());
        using var response=await Client.PostAsJsonAsync("/api/users",Request("created.user",KahradPricingApiWebAppFactory.TestPassword,[roleId]));
        Assert.Equal(HttpStatusCode.Created,response.StatusCode);var id=await ReadResultAsync<Guid>(response);
        Assert.NotEqual(Guid.Empty,id);
        var user=await ReadDatabaseAsync<IdentityDbContext,Kahrad.Pricing.Identity.Domain.Entities.ApplicationUser>(db=>db.Users.AsNoTracking().SingleAsync(x=>x.Id==id));
        Assert.Equal("CREATED.USER",user.NormalizedUserName);Assert.True(!string.IsNullOrEmpty(user.PasswordHash)&&user.PasswordHash!=KahradPricingApiWebAppFactory.TestPassword);
        Assert.True(await ReadDatabaseAsync<IdentityDbContext,bool>(db=>db.UserRoles.AnyAsync(x=>x.UserId==id&&x.RoleId==roleId)));
        Assert.True(await ReadDatabaseAsync<IdentityDbContext,bool>(db=>db.AuditLogs.AnyAsync(x=>x.Action=="UserCreated")));
    }
    [SqlServerFact]
    public async Task Weak_password_is_rejected_without_a_user_record()
    {
        using var response=await Client.PostAsJsonAsync("/api/users",Request("weak.user","123456",[]));
        Assert.Equal(HttpStatusCode.BadRequest,response.StatusCode);
        Assert.False(await ReadDatabaseAsync<IdentityDbContext,bool>(db=>db.Users.AnyAsync(x=>x.UserName=="weak.user")));
    }
    [SqlServerFact]
    public async Task Admin_cannot_create_a_superadmin()
    {
        var roleId=await ReadDatabaseAsync<IdentityDbContext,Guid>(db=>db.Roles.Where(x=>x.Name==RoleConstants.SuperAdmin).Select(x=>x.Id).SingleAsync());
        using var admin=await Factory.CreateAuthenticatedClientAsync(RoleConstants.Admin);
        using var response=await admin.PostAsJsonAsync("/api/users",Request("forbidden.user",KahradPricingApiWebAppFactory.TestPassword,[roleId]));
        Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);
        Assert.False(await ReadDatabaseAsync<IdentityDbContext,bool>(db=>db.Users.AnyAsync(x=>x.UserName=="forbidden.user")));
    }
    [SqlServerFact]
    public async Task Anonymous_user_creation_requires_authentication()
    {
        using var anonymous=AnonymousClient();using var response=await anonymous.PostAsJsonAsync("/api/users",Request("anonymous.user",KahradPricingApiWebAppFactory.TestPassword,[]));
        Assert.Equal(HttpStatusCode.Unauthorized,response.StatusCode);
    }
    private static CreateUserRequest Request(string username,string password,Guid[] roles)=>new()
    {Username=username,Password=password,FirstName="Created",LastName="User",PhoneNumber="09121234567",RoleIds=roles};
}
