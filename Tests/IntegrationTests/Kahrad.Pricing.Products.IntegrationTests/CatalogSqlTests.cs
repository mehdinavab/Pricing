using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.DTOs.Auditing;
using Kahrad.Pricing.Products.Application;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Kahrad.Pricing.Products.Domain.Entities;
using Kahrad.Pricing.Products.Domain.Enums;
using Kahrad.Pricing.Products.Infrastructure;
using Kahrad.Pricing.Products.Infrastructure.Persistence;
using Kahrad.Pricing.Products.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using Xunit;
namespace Kahrad.Pricing.Products.IntegrationTests;
public sealed class CatalogSqlTests
{
    private const string TestKey = "catalog-integration-test-signing-key-01234567890123456789";
    [CatalogSqlFact]
    public async Task Migration_localization_manual_admin_constraints_and_legacy_behavior()
    {
        var databaseName = "KahradCatalogTests_" + Guid.NewGuid().ToString("N");
        var builder = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable("CATALOG_TEST_SQL_SERVER")!)
        { InitialCatalog = databaseName, TrustServerCertificate = true };
        var connection = builder.ConnectionString;
        await using var context = new ProductsDbContext(new DbContextOptionsBuilder<ProductsDbContext>().UseSqlServer(connection).Options);
        try
        {
            // Migrate legacy schema, load representative Persian rows, then migrate forward.
            await context.GetService<IMigrator>().MigrateAsync("20261104101902_InitialProductsSchema");
            await context.Database.ExecuteSqlRawAsync("""
                INSERT INTO products.ProductGroups (Name,Code,IsActive,PublicId,IsDeleted,CreatedAtUtc)
                VALUES (N'گروه',N'TEST-GROUP',1,NEWID(),0,SYSUTCDATETIME());
                INSERT INTO products.Products (Name,Code,GroupId,BasePrice,IsActive,PublicId,IsDeleted,CreatedAtUtc)
                SELECT N'محصول قدیمی',N'LEGACY-PRODUCT',Id,100,1,NEWID(),0,SYSUTCDATETIME() FROM products.ProductGroups;
                INSERT INTO products.ProductFeatures (ProductId,Name,Value,HasPriceEffect,PriceEffectAmount,PublicId,IsDeleted,CreatedAtUtc)
                SELECT Id,N'ضد آب',N'بله',1,5,NEWID(),0,SYSUTCDATETIME() FROM products.Products;
                """);
            await context.Database.MigrateAsync();
            var languages = await context.Languages.ToListAsync();
            var fa = languages.Single(x => x.Code == "fa"); var en = languages.Single(x => x.Code == "en");
            var legacy = await context.Products.SingleAsync();
            Assert.Equal("محصول قدیمی", (await context.ProductTranslations.SingleAsync()).Name);
            Assert.Equal(5, (await context.ProductFeatures.SingleAsync()).PriceEffectAmount);
            Assert.NotNull((await context.ProductFeatures.SingleAsync()).FeatureId);
            Assert.Equal("ضد آب", (await context.FeatureTranslations.SingleAsync()).Name);
            context.ChangeTracker.Clear();
            var repository = new CatalogRepository(context, NullLogger<CatalogRepository>.Instance);
            await repository.UpsertTranslationAsync(TranslationTarget.Product, new(legacy.Id, "en", "Legacy product"), null, default);
            var featureId = (await context.ProductFeatures.SingleAsync()).FeatureId!.Value;
            await repository.UpsertTranslationAsync(TranslationTarget.Feature, new(featureId, "en", "Waterproof"), null, default);
            var shared = await repository.GetFeatureAsync(featureId, default);
            var products = new ProductRepository(context);
            var second = new Product("محصول دوم", "SECOND", legacy.GroupId, 200);
            await products.AddAsync(second);
            var assignment = new ProductFeature(second.Id, shared!.Name, "بله", false); assignment.AssignDefinition(featureId);
            await products.AddFeatureAsync(assignment);
            var textSpec = await repository.CreateDefinitionAsync(true, new("MATERIAL", "جنس", SpecificationValueType.Text), null, default);
            var numericSpec = await repository.CreateDefinitionAsync(true, new("WEIGHT", "وزن", SpecificationValueType.Number, "kg"), null, default);
            var booleanSpec = await repository.CreateDefinitionAsync(true, new("ENABLED", "فعال", SpecificationValueType.Boolean), null, default);
            var textAssignment = await repository.AssignSpecificationAsync(new(legacy.Id, textSpec, TextValue: "چوب"), null, default);
            await repository.AssignSpecificationAsync(new(second.Id, textSpec, TextValue: "چوب"), null, default);
            await repository.AssignSpecificationAsync(new(legacy.Id, numericSpec, NumberValue: 12.5m), null, default);
            await repository.AssignSpecificationAsync(new(legacy.Id, booleanSpec, BooleanValue: false), null, default);
            await repository.UpsertTranslationAsync(TranslationTarget.Specification, new(textSpec, "en", "Material"), null, default);
            await repository.UpsertTranslationAsync(TranslationTarget.ProductSpecification, new(textAssignment, "en", "Wood"), null, default);
            foreach (var code in new[] { "fa", "ar", "en" })
            {
                var language = languages.Single(x => x.Code == code);
                var result = await repository.GetDetailsAsync(legacy.Id, new(language.Id, code, fa.Id, "fa"), default);
                Assert.NotNull(result);
                Assert.Equal(code == "en" ? "Legacy product" : "محصول قدیمی", result.Name);
                Assert.Equal(code == "en" ? "Waterproof" : "ضد آب", Assert.Single(result.Features).Name);
                Assert.Equal(code == "en" ? "Wood" : "چوب", result.Specifications.Single(x => x.SpecificationId == textSpec).TextValue);
                Assert.Equal(12.5m, result.Specifications.Single(x => x.SpecificationId == numericSpec).NumberValue);
                Assert.False(result.Specifications.Single(x => x.SpecificationId == booleanSpec).BooleanValue);
            }
            Assert.Equal("Waterproof", Assert.Single((await repository.GetDetailsAsync(second.Id, new(en.Id, "en", fa.Id, "fa"), default))!.Features).Name);
            Assert.Equal("Material", Assert.Single((await repository.GetDetailsAsync(second.Id, new(en.Id, "en", fa.Id, "fa"), default))!.Specifications).Name);
            var numericAssignment = (await repository.GetAssignmentsAsync(new(legacy.Id), default)).Single(x => x.SpecificationId == numericSpec);
            await Assert.ThrowsAsync<Kahrad.Pricing.Products.Domain.Exceptions.CatalogRuleException>(() => repository.UpsertTranslationAsync(TranslationTarget.ProductSpecification, new(numericAssignment.Id,"en","twelve"), null, default));
            context.ChangeTracker.Clear();
            var page = await repository.SearchAsync(new() { PageSize = 1, Name = "product" }, new(en.Id, "en", fa.Id, "fa"), default);
            Assert.Equal(1, page.TotalCount); Assert.Single(page.Data);
            Assert.Equal(2, (await repository.SearchAsync(new() { PageSize = 1 }, new(en.Id,"en",fa.Id,"fa"), default)).TotalCount);
            var archivedGroup = new ProductGroup("گروه حذف شده", "ARCHIVED");
            context.ProductGroups.Add(archivedGroup); await context.SaveChangesAsync();
            var hiddenProduct = new Product("محصول قدیمی", "HIDDEN", archivedGroup.Id, 1);
            await products.AddAsync(hiddenProduct); archivedGroup.SetDeleted(null, DateTime.UtcNow); await context.SaveChangesAsync();
            Assert.Equal(2, (await repository.SearchAsync(new() { PageSize = 100 }, new(en.Id,"en",fa.Id,"fa"), default)).TotalCount);
            // SQL Server, rather than InMemory, verifies composite uniqueness and foreign keys.
            context.ChangeTracker.Clear();
            context.ProductTranslations.Add(new ProductTranslation(legacy.Id, en.Id, "Duplicate"));
            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync()); context.ChangeTracker.Clear();
            context.ProductTranslations.Add(new ProductTranslation(long.MaxValue, en.Id, "Missing parent"));
            await Assert.ThrowsAsync<DbUpdateException>(() => context.SaveChangesAsync()); context.ChangeTracker.Clear();
            Assert.Equal(12.5m, (await context.ProductSpecifications.SingleAsync(x => x.SpecificationId == numericSpec)).NumberValue);
            await VerifyApiAsync(connection, legacy.Id, featureId);
            // Existing service creates/edits products and estimates the same numerical price.
            var services = new ServiceCollection(); services.AddLogging(); services.AddProductsApplication();
            services.AddProductsInfrastructure(new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string,string?> { ["ConnectionStrings:ProductsDb"] = connection }).Build());
            await using var provider = services.BuildServiceProvider(); await using var scope = provider.CreateAsyncScope();
            var productService = scope.ServiceProvider.GetRequiredService<IProductService>();
            var estimate = await productService.CalculateEstimatedPriceAsync(legacy.Id);
            Assert.Equal(105m, estimate.EstimatedTotalPrice);
            var createdId = await productService.CreateAsync(new CreateProductRequest { Name = "محصول تازه", Code = "NEW", GroupId = legacy.GroupId, BasePrice = 99 });
            await productService.UpdateAsync(createdId, new UpdateProductRequest { Name = "نام ویرایش شده", Code = "NEW", GroupId = legacy.GroupId, BasePrice = 99 });
            context.ChangeTracker.Clear();
            Assert.Equal("نام ویرایش شده", (await context.ProductTranslations.SingleAsync(x => x.ProductId == createdId && x.LanguageId == fa.Id)).Name);
            context.ChangeTracker.Clear();
            var currentEn = (await repository.GetLanguagesAsync(default)).Single(x => x.Code == "en");
            await Assert.ThrowsAsync<Kahrad.Pricing.Products.Domain.Exceptions.CatalogRuleException>(() => repository.UpsertLanguageAsync(new("en", "English", true, true, currentEn.RowVersion), null, default));
            foreach (var active in await context.Products.AsNoTracking().Where(x => x.IsActive).ToListAsync())
            {
                if (!await context.ProductTranslations.AnyAsync(t => t.ProductId == active.Id && t.LanguageId == en.Id))
                    await repository.UpsertTranslationAsync(TranslationTarget.Product, new(active.Id, "en", "Test product " + active.Id), null, default);
            }
            currentEn = (await repository.GetLanguagesAsync(default)).Single(x => x.Code == "en");
            await repository.UpsertLanguageAsync(new("en", "English", true, true, currentEn.RowVersion), null, default);
            context.ChangeTracker.Clear();
            Assert.Equal("en", (await context.Languages.SingleAsync(x => x.IsDefault)).Code);
        }
        finally
        {
            // The target is generated here, never supplied as an application database name.
            Assert.StartsWith("KahradCatalogTests_", databaseName);
            await context.Database.EnsureDeletedAsync();
        }
    }
    private static async Task VerifyApiAsync(string connection, long productId, long featureId)
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName,"Kahrad.Pricing.slnx"))) root = root.Parent;
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(web =>
        {
            web.UseEnvironment("Development"); web.UseContentRoot(Path.Combine(root!.FullName,"Src/Hosts/Kahrad.Pricing.Api"));
            web.ConfigureAppConfiguration((_, configuration) => configuration.AddInMemoryCollection(new Dictionary<string,string?>
            {
                ["ConnectionStrings:DefaultConnection"] = connection, ["ConnectionStrings:ProductsDb"] = connection,
                ["Jwt:Key"] = TestKey, ["Jwt:Issuer"] = "CatalogTests", ["Jwt:Audience"] = "CatalogTests",
                ["Logging:LogLevel:Default"] = "Warning",
                ["Jwt:AccessTokenLifetimeMinutes"] = "15", ["Jwt:RefreshTokenLifetimeDays"] = "1"
            }));
            web.ConfigureServices(services =>
            {
                services.RemoveAll<IAuditService>(); services.AddSingleton<IAuditService, AuditStub>();
                services.RemoveAll<ProductsDbContext>(); services.RemoveAll<DbContextOptions<ProductsDbContext>>();
                services.AddDbContext<ProductsDbContext>(options => options.UseSqlServer(connection));
                services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters.ValidIssuer = "CatalogTests";
                    options.TokenValidationParameters.ValidAudience = "CatalogTests";
                    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestKey));
                });
            });
        });
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
        var anonymous = await client.PostAsJsonAsync("/api/products/admin/languages/list", new { });
        Assert.Equal(HttpStatusCode.Unauthorized, anonymous.StatusCode);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token("Contributor"));
        Assert.Equal(HttpStatusCode.Forbidden, (await client.PostAsJsonAsync("/api/products/admin/languages/list", new { })).StatusCode);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token("Admin"));
        var upsert = await client.PostAsJsonAsync("/api/products/admin/products/translations/upsert", new TranslationRequest(productId,"ar","منتج عربي"));
        Assert.Equal(HttpStatusCode.NoContent, upsert.StatusCode);
        var list = await client.PostAsJsonAsync("/api/products/admin/products/translations/list", new TranslationListRequest(productId));
        var translations = (await list.Content.ReadFromJsonAsync<List<TranslationDto>>())!;
        Assert.Contains(translations, x => x.LanguageCode == "ar" && x.Text == "منتج عربي");
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync("/api/products/admin/products/translations/upsert", new TranslationRequest(productId,"ar","stale update"))).StatusCode);
        var languagesResponse = await client.PostAsJsonAsync("/api/products/admin/languages/list", new { });
        var languages = (await languagesResponse.Content.ReadFromJsonAsync<List<LanguageDto>>())!;
        var fa = languages.Single(x=>x.IsDefault);
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync("/api/products/admin/languages/upsert", new LanguageRequest(fa.Code,fa.Name,false,false,fa.RowVersion))).StatusCode);
        client.DefaultRequestHeaders.Authorization = null;
        client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("ar");
        var details = await client.PostAsJsonAsync("/api/products/details", new CatalogDetailsRequest(productId));
        Assert.Equal(HttpStatusCode.OK, details.StatusCode);
        Assert.Equal("ar", Assert.Single(details.Content.Headers.ContentLanguage));
        Assert.Contains("Accept-Language", details.Headers.Vary);
        var product = (await details.Content.ReadFromJsonAsync<ProductDetailDto>())!;
        Assert.Equal("منتج عربي", product.Name);
        Assert.Equal("ضد آب", Assert.Single(product.Features).Name); // Independent fallback.
        var search = await client.PostAsJsonAsync("/api/products/search", new CatalogSearchRequest { PageSize = 1 });
        Assert.Equal(HttpStatusCode.OK, search.StatusCode);
        var swagger = await client.GetStringAsync("/swagger/v1/swagger.json");
        using var document = JsonDocument.Parse(swagger);
        var paths = document.RootElement.GetProperty("paths");
        var detailsOperation = paths.GetProperty("/api/products/details").GetProperty("post");
        Assert.Contains(detailsOperation.GetProperty("parameters").EnumerateArray(), x => x.GetProperty("name").GetString() == "Accept-Language");
        Assert.True(paths.TryGetProperty("/api/products/admin/products/translations/upsert",out _));
        var builtInOpenApi = await client.GetAsync("/openapi/v1.json");
        Assert.Equal(HttpStatusCode.OK, builtInOpenApi.StatusCode);
        using var builtin = JsonDocument.Parse(await builtInOpenApi.Content.ReadAsStringAsync());
        var adminSecurity = builtin.RootElement.GetProperty("paths").GetProperty("/api/products/admin/languages/list").GetProperty("post").GetProperty("security");
        Assert.NotEmpty(adminSecurity.EnumerateArray());
        Assert.Empty(detailsOperation.GetProperty("security").EnumerateArray());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token("Admin"));
        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsJsonAsync("/api/products/admin/languages/upsert",new LanguageRequest("de","Deutsch"))).StatusCode);
        Assert.Equal(HttpStatusCode.OK,(await client.PostAsJsonAsync("/api/products/admin/product-specifications/list",new SpecificationAssignmentsRequest(productId))).StatusCode);
        var addShared = await client.PostAsJsonAsync($"/api/products/{productId}/features", new { FeatureId = featureId, Value = "بله", HasPriceEffect = false });
        Assert.True(addShared.StatusCode == HttpStatusCode.Created, await addShared.Content.ReadAsStringAsync());
    }
    private static string Token(string role) => new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
        "CatalogTests", "CatalogTests", [new Claim(ClaimTypes.Role,role), new Claim(ClaimTypes.NameIdentifier,Guid.NewGuid().ToString())],
        expires: DateTime.UtcNow.AddMinutes(5), signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TestKey)), SecurityAlgorithms.HmacSha256)));
    private sealed class AuditStub : IAuditService
    {
        public Task LogAsync(string action,string entityType,string? entityId=null,string? description=null,Guid? actorUserId=null,string? ipAddress=null,string? correlationId=null,CancellationToken cancellationToken=default) => Task.CompletedTask;
        public Task<AuditLogDto?> GetByIdAsync(long id,CancellationToken cancellationToken=default) => throw new NotSupportedException();
        public Task<BaseResponsePaginated<AuditLogDto>> GetPagedAsync(AuditLogQueryDto filter,CancellationToken cancellationToken=default) => throw new NotSupportedException();
    }
}
public sealed class CatalogSqlFactAttribute : FactAttribute
{
    public CatalogSqlFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("CATALOG_TEST_SQL_SERVER")))
            Skip = "Set CATALOG_TEST_SQL_SERVER to an authorized local/test SQL Server; the test creates and removes its own isolated database.";
    }
}
