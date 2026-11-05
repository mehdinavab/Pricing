using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.BuildingBlocks.Infrastructure.Hosting;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Auditing;
using Kahrad.Pricing.Identity.Application.DTOs.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Xunit;
namespace Kahrad.Pricing.Hosts.IntegrationTests;
public sealed class HostSecurityTests
{
    private const string Key = "host-security-tests-only-signing-key-01234567890123456789";
    private static readonly Guid ProtectedUser = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid OtherUser = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private static WebApplicationFactory<Program> Factory(UsersStub users)
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while(root is not null && !File.Exists(Path.Combine(root.FullName,"Kahrad.Pricing.slnx"))) root=root.Parent;
        return new WebApplicationFactory<Program>().WithWebHostBuilder(web =>
        {
            web.UseEnvironment("Development");
            web.UseContentRoot(Path.Combine(root!.FullName,"Src/Hosts/Kahrad.Pricing.Api"));
            web.ConfigureServices(services =>
            {
                services.RemoveAll<IUserService>();services.AddSingleton<IUserService>(users);
                services.RemoveAll<IAuditService>();services.AddSingleton<IAuditService,AuditStub>();
                services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme,options =>
                {
                    options.TokenValidationParameters.ValidIssuer="HostTests";options.TokenValidationParameters.ValidAudience="HostTests";
                    options.TokenValidationParameters.IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
                });
                services.Configure<HealthCheckServiceOptions>(options =>
                {
                    options.Registrations.Clear();
                    options.Registrations.Add(new HealthCheckRegistration("sql",new RequiredSqlHealthCheck("invalid-private-configuration"),null,["ready"]));
                });
            });
        });
    }
    [Theory]
    [InlineData("reset-password","POST")]
    [InlineData("profile","PUT")]
    [InlineData("status","PATCH")]
    [InlineData("remove-role","DELETE")]
    [InlineData("assign-role","POST")]
    public async Task Admin_cannot_mutate_a_superadmin(string action,string method)
    {
        var users=new UsersStub();using var factory=Factory(users);using var client=factory.CreateClient();
        client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",Token("Admin"));
        using var request=Request(action,method,ProtectedUser);
        var response=await client.SendAsync(request);
        Assert.Equal(HttpStatusCode.Forbidden,response.StatusCode);Assert.Equal(0,users.Mutations);
    }
    [Theory]
    [InlineData("SuperAdmin",true)]
    [InlineData("Admin",false)]
    public async Task Authorized_account_resets_still_work(string actor,bool protectedTarget)
    {
        var users=new UsersStub();using var factory=Factory(users);using var client=factory.CreateClient();
        client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",Token(actor));
        using var request=Request("reset-password","POST",protectedTarget?ProtectedUser:OtherUser);
        Assert.Equal(HttpStatusCode.NoContent,(await client.SendAsync(request)).StatusCode);Assert.Equal(1,users.Mutations);
    }
    [Theory]
    [InlineData(null,HttpStatusCode.Unauthorized)]
    [InlineData("Contributor",HttpStatusCode.Forbidden)]
    public async Task Unauthenticated_or_unprivileged_writes_are_rejected(string? role,HttpStatusCode status)
    {
        var users=new UsersStub();using var factory=Factory(users);using var client=factory.CreateClient();
        if(role is not null)client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",Token(role));
        using var request=Request("reset-password","POST",OtherUser);
        Assert.Equal(status,(await client.SendAsync(request)).StatusCode);Assert.Equal(0,users.Mutations);
    }
    [Theory]
    [InlineData(null,HttpStatusCode.Unauthorized)]
    [InlineData("Contributor",HttpStatusCode.Forbidden)]
    public async Task Raw_otp_sends_require_administration_privileges(string? role,HttpStatusCode status)
    {
        using var factory=Factory(new());using var client=factory.CreateClient();
        if(role is not null)client.DefaultRequestHeaders.Authorization=new AuthenticationHeaderValue("Bearer",Token(role));
        var result=await client.PostAsJsonAsync("/api/sms/send-otp",new { PhoneNumber="09121234567",Code="456789" });
        Assert.Equal(status,result.StatusCode);
    }
    [Fact]
    public async Task Liveness_is_independent_and_readiness_hides_private_configuration()
    {
        using var factory=Factory(new());using var client=factory.CreateClient();
        var live=await client.GetAsync("/health/live");Assert.Equal(HttpStatusCode.OK,live.StatusCode);
        var ready=await client.GetAsync("/health/ready");Assert.Equal(HttpStatusCode.ServiceUnavailable,ready.StatusCode);
        var body=await ready.Content.ReadAsStringAsync();Assert.Equal("{\"status\":\"Unhealthy\"}",body);
        Assert.DoesNotContain("private",body,StringComparison.OrdinalIgnoreCase);Assert.DoesNotContain("sql",body,StringComparison.OrdinalIgnoreCase);
        Assert.Contains("no-store",ready.Headers.CacheControl!.ToString());
    }
    [Theory]
    [InlineData("valid-correlation_123",true)]
    [InlineData("invalid.correlation",false)]
    public async Task Correlation_headers_are_validated(string value,bool retained)
    {
        using var factory=Factory(new());using var client=factory.CreateClient();client.DefaultRequestHeaders.Add("X-Correlation-ID",value);
        var result=await client.GetAsync("/health/live");var correlation=Assert.Single(result.Headers.GetValues("X-Correlation-ID"));
        Assert.Equal(retained,value==correlation);
    }
    private static HttpRequestMessage Request(string action,string method,Guid userId)
    {
        var path=action switch {"profile"=>$"/api/users/{userId}","remove-role" or "assign-role"=>$"/api/users/{userId}/roles/{Guid.Empty}",_=>$"/api/users/{userId}/{action}"};
        var request=new HttpRequestMessage(new HttpMethod(method),path);
        if(action=="reset-password")request.Content=JsonContent.Create(new{NewPassword="Valid-test-password123!"});
        if(action=="profile")request.Content=JsonContent.Create(new{FirstName="Test",LastName="User",Email="test@example.test",PhoneNumber="09121234567"});
        if(action=="status")request.Content=JsonContent.Create(new{IsActive=false});
        return request;
    }
    private static string Token(string role)=>new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken("HostTests","HostTests",
        [new Claim(ClaimTypes.Role,role),new Claim(ClaimTypes.NameIdentifier,Guid.NewGuid().ToString())],expires:DateTime.UtcNow.AddMinutes(5),
        signingCredentials:new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key)),SecurityAlgorithms.HmacSha256)));
    public sealed class UsersStub:IUserService
    {
        public int Mutations{get;private set;}
        private Task Mutate(){Mutations++;return Task.CompletedTask;}
        public Task<IReadOnlyCollection<string>> GetUserRolesAsync(Guid userId,CancellationToken cancellationToken=default)=>Task.FromResult<IReadOnlyCollection<string>>(userId==ProtectedUser?["SuperAdmin"]:[]);
        public Task ResetPasswordAsync(Guid id,ResetPasswordRequest request,CancellationToken ct=default)=>Mutate();
        public Task UpdateAsync(Guid id,UpdateUserRequest request,CancellationToken ct=default)=>Mutate();
        public Task SetActiveStatusAsync(Guid id,bool active,CancellationToken ct=default)=>Mutate();
        public Task AssignRoleAsync(Guid id,Guid role,CancellationToken ct=default)=>Mutate();
        public Task RemoveRoleAsync(Guid id,Guid role,CancellationToken ct=default)=>Mutate();
        public Task ChangePasswordAsync(Guid id,ChangePasswordRequest request,CancellationToken ct=default)=>throw new NotSupportedException();
        public Task<Guid>CreateAsync(CreateUserRequest request,CancellationToken ct=default)=>throw new NotSupportedException();
        public Task<UserDto?>GetByIdAsync(Guid id,CancellationToken ct=default)=>throw new NotSupportedException();
        public Task<UserDto?>GetByUsernameAsync(string name,CancellationToken ct=default)=>throw new NotSupportedException();
        public Task<BaseResponsePaginated<UserDto>>GetPagedAsync(UserQueryDto query,CancellationToken ct=default)=>throw new NotSupportedException();
    }
    private sealed class AuditStub:IAuditService
    {
        public Task LogAsync(string action,string type,string? id=null,string? description=null,Guid? actor=null,string? ip=null,string? correlation=null,CancellationToken ct=default)=>Task.CompletedTask;
        public Task<AuditLogDto?>GetByIdAsync(long id,CancellationToken ct=default)=>throw new NotSupportedException();
        public Task<BaseResponsePaginated<AuditLogDto>>GetPagedAsync(AuditLogQueryDto query,CancellationToken ct=default)=>throw new NotSupportedException();
    }
}
