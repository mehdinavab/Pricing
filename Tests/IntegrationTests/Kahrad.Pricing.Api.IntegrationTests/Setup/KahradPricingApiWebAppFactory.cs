using Kahrad.Pricing.Brands.Infrastructure.Persistence;
using Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;
using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Kahrad.Pricing.Sales.Infrastructure.Persistence;
using Kahrad.Pricing.Representatives.Infrastructure.Persistence;
using Kahrad.Pricing.Pricing.Infrastructure.Persistence;
using Kahrad.Pricing.Guarantees.Infrastructure.Persistence;
using Kahrad.Pricing.Identity.Infrastructure.Persistence;
using Kahrad.Pricing.Identity.Infrastructure.Services.models;
using Kahrad.Pricing.Products.Infrastructure.Persistence;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Kahrad.Pricing.Sms.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Respawn;
using Respawn.Graph;
using System.Text;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.Setup;

public sealed class KahradPricingApiWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public const string ConnectionVariable = "KAHRAD_TEST_SQL_SERVER";
    public const string TestPassword = "Integration-test-password123!";
    public const string TestUsername = "integration.superadmin";
    private const string DatabasePrefix = "KahradApiTests_";
    private const string SigningKey = "kahrad-api-integration-tests-only-key-01234567890123456789";
    private readonly string _databaseName = DatabasePrefix + Guid.NewGuid().ToString("N");
    private string? _connectionString;
    private Respawner? _respawner;
    private bool _databaseCreated;
    private bool _disposed;
    public RecordingSmsProvider SmsProvider { get; } = new();
    public RecordingAccountingProvider AccountingProvider { get; } = new();
    public IReadOnlyDictionary<string,string> MigratedLanguageNames { get; private set; } = new Dictionary<string,string>();
    public string ConnectionString => _connectionString ?? throw new InvalidOperationException("The isolated SQL test database has not been initialized.");

    public async Task InitializeAsync()
    {
        var configured = Environment.GetEnvironmentVariable(ConnectionVariable);
        if (string.IsNullOrWhiteSpace(configured)) return;
        var settings = new SqlConnectionStringBuilder(configured) { InitialCatalog = _databaseName };
        if (!string.IsNullOrWhiteSpace(settings.AttachDBFilename))
            throw new InvalidOperationException("Integration tests cannot attach an existing database file.");
        _connectionString = settings.ConnectionString;
        EnsureOwnedDatabase();
        _databaseCreated = true;
        try
        {
            await using (var db = new IdentityDbContext(new DbContextOptionsBuilder<IdentityDbContext>().UseSqlServer(ConnectionString).Options)) await db.Database.MigrateAsync();
            await using (var db = new BrandsDbContext(new DbContextOptionsBuilder<BrandsDbContext>().UseSqlServer(ConnectionString).Options)) await db.Database.MigrateAsync();
            await using (var db = new ProductsDbContext(new DbContextOptionsBuilder<ProductsDbContext>().UseSqlServer(ConnectionString).Options))
            {
                await db.Database.MigrateAsync();
                MigratedLanguageNames = await db.Languages.AsNoTracking().ToDictionaryAsync(x => x.Code,x => x.Name);
            }
            await using (var db = new GuaranteesDbContext(new DbContextOptionsBuilder<GuaranteesDbContext>().UseSqlServer(ConnectionString).Options)) await db.Database.MigrateAsync();
            await using (var db = new SmsDbContext(new DbContextOptionsBuilder<SmsDbContext>().UseSqlServer(ConnectionString).Options)) await db.Database.MigrateAsync();
            await using (var db = new AccountingDbContext(new DbContextOptionsBuilder<AccountingDbContext>().UseSqlServer(ConnectionString).Options)) await db.Database.MigrateAsync();
            await using (var db = new SalesDbContext(new DbContextOptionsBuilder<SalesDbContext>().UseSqlServer(ConnectionString).Options)) await db.Database.MigrateAsync();
            await using (var db = new RepresentativesDbContext(new DbContextOptionsBuilder<RepresentativesDbContext>().UseSqlServer(ConnectionString).Options)) await db.Database.MigrateAsync();
            await using (var db = new PricingDbContext(new DbContextOptionsBuilder<PricingDbContext>().UseSqlServer(ConnectionString).Options)) await db.Database.MigrateAsync();
            await using var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync();
            _respawner = await Respawner.CreateAsync(connection,new RespawnerOptions
            {
                DbAdapter = DbAdapter.SqlServer,
                SchemasToInclude = ["identity","brands","products","guarantees","sms","accounting","sales","representatives","pricing"],
                TablesToIgnore = [new Table("dbo","__EFMigrationsHistory")]
            });
        }
        catch
        {
            await DeleteOwnedDatabaseAsync();
            throw;
        }
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName,"Kahrad.Pricing.slnx"))) root = root.Parent;
        builder.UseContentRoot(Path.Combine(root?.FullName ?? throw new InvalidOperationException("Solution root was not found."),"Src/Hosts/Kahrad.Pricing.Api"));
        builder.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(new Dictionary<string,string?>
        {
            ["ConnectionStrings:DefaultConnection"] = ConnectionString,
            ["ConnectionStrings:IdentityDb"] = ConnectionString,
            ["ConnectionStrings:BrandsDb"] = ConnectionString,
            ["ConnectionStrings:ProductsDb"] = ConnectionString,
            ["ConnectionStrings:GuaranteesDb"] = ConnectionString,
            ["ConnectionStrings:SmsDb"] = ConnectionString,
            ["ConnectionStrings:AccountingDb"] = ConnectionString,
            ["ConnectionStrings:SalesDb"] = ConnectionString,
            ["ConnectionStrings:RepresentativesDb"] = ConnectionString,
            ["ConnectionStrings:PricingDb"] = ConnectionString,
            ["Jwt:Key"] = SigningKey, ["Jwt:Issuer"] = "KahradIntegrationTests", ["Jwt:Audience"] = "KahradIntegrationTests",
            ["Jwt:AccessTokenLifetimeMinutes"] = "15", ["Jwt:RefreshTokenLifetimeDays"] = "1",
            ["Serilog:MinimumLevel:Default"] = "Fatal"
        }));
        builder.ConfigureTestServices(services =>
        {
            ReplaceContext<IdentityDbContext>(services);
            ReplaceContext<BrandsDbContext>(services);
            ReplaceContext<ProductsDbContext>(services);
            ReplaceContext<GuaranteesDbContext>(services);
            ReplaceContext<SmsDbContext>(services);
            ReplaceContext<AccountingDbContext>(services);
            ReplaceContext<SalesDbContext>(services);
            ReplaceContext<RepresentativesDbContext>(services);
            ReplaceContext<PricingDbContext>(services);
            services.AddDataProtection().UseEphemeralDataProtectionProvider();
            services.PostConfigure<JwtOptions>(options =>
            {
                options.Key = SigningKey; options.Issuer = "KahradIntegrationTests"; options.Audience = "KahradIntegrationTests";
                options.AccessTokenLifetimeMinutes = 15; options.RefreshTokenLifetimeDays = 1;
            });
            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters.ValidIssuer = "KahradIntegrationTests";
                options.TokenValidationParameters.ValidAudience = "KahradIntegrationTests";
                options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SigningKey));
            });
            services.RemoveAll<ISmsProviderResolver>();
            services.AddSingleton<ISmsProviderResolver>(new RecordingSmsProviderResolver(SmsProvider));
            services.RemoveAll<IAccountingProviderClient>();
            services.AddSingleton<IAccountingProviderClient>(AccountingProvider);
        });
    }

    private void ReplaceContext<TContext>(IServiceCollection services) where TContext : DbContext
    {
        services.RemoveAll<TContext>(); services.RemoveAll<DbContextOptions<TContext>>();
        services.RemoveAll<IDbContextOptionsConfiguration<TContext>>();
        services.AddDbContext<TContext>(options => options.UseSqlServer(ConnectionString));
    }

    public async Task ResetDbAsync()
    {
        EnsureOwnedDatabase();
        if (_respawner is null) throw new InvalidOperationException("Database reset was requested before initialization.");
        await using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        await _respawner.ResetAsync(connection);
        SmsProvider.Reset();
        AccountingProvider.Reset();
        await this.PopulateCatalogAndSmsAsync();
    }

    private void EnsureOwnedDatabase()
    {
        var actual = new SqlConnectionStringBuilder(ConnectionString).InitialCatalog;
        if (!actual.Equals(_databaseName,StringComparison.Ordinal) || !actual.StartsWith(DatabasePrefix,StringComparison.Ordinal) ||
            !Guid.TryParseExact(actual[DatabasePrefix.Length..],"N",out _))
            throw new InvalidOperationException("Refusing to reset or delete a database not generated by this test factory.");
    }

    private async Task DeleteOwnedDatabaseAsync()
    {
        if (!_databaseCreated) return;
        EnsureOwnedDatabase();
        await using var context = new IdentityDbContext(new DbContextOptionsBuilder<IdentityDbContext>().UseSqlServer(ConnectionString).Options);
        await context.Database.EnsureDeletedAsync();
        _databaseCreated = false;
    }

    async Task IAsyncLifetime.DisposeAsync() => await DisposeAsync();
    public override async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;
        try { await base.DisposeAsync(); }
        finally { await DeleteOwnedDatabaseAsync(); }
    }
}
