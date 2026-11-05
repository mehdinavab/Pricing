using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Identity.Infrastructure.Persistence;
using Kahrad.Pricing.Products.Infrastructure.Persistence;
using Kahrad.Pricing.Sms.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Infrastructure;
public sealed class DatabaseResetTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Reset_removes_business_and_authentication_rows_but_retains_baseline_configuration()
    {
        await Factory.CreateProductAsync();
        Assert.True(await ReadDatabaseAsync<ProductsDbContext,bool>(db=>db.Products.AnyAsync()));
        await Factory.ResetDbAsync();
        Assert.False(await ReadDatabaseAsync<ProductsDbContext,bool>(db=>db.Products.AnyAsync()));
        Assert.False(await ReadDatabaseAsync<IdentityDbContext,bool>(db=>db.Users.AnyAsync()));
        Assert.False(await ReadDatabaseAsync<IdentityDbContext,bool>(db=>db.RefreshTokens.AnyAsync()));
        Assert.Equal(3,await ReadDatabaseAsync<ProductsDbContext,int>(db=>db.Languages.CountAsync()));
        Assert.Equal(1,await ReadDatabaseAsync<SmsDbContext,int>(db=>db.ModuleConfigurations.CountAsync()));
    }
    [SqlServerFact]
    public void Migrated_language_display_names_preserve_real_unicode()
    {
        Assert.Equal(CatalogSetupExtensionMethods.PersianLanguageName,Factory.MigratedLanguageNames["fa"]);
        Assert.Equal(CatalogSetupExtensionMethods.ArabicLanguageName,Factory.MigratedLanguageNames["ar"]);
    }
}
