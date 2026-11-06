using System.Text;
using MAHAK.Integration.Api.Configuration;
using MAHAK.Integration.Api.Data;
using MAHAK.Integration.Api.Security;
using MAHAK.Integration.Tests.TestSupport;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Xunit;

namespace MAHAK.Integration.Tests.Security;

public sealed class ConfigurationSecurityTests
{
    [Fact]
    public void Json_configuration_routes_each_key_to_only_its_catalog()
    {
        const string json = """
            {
              "Mahak": {
                "Brands": {
                  "Kahrad": {
                    "Enabled": true,
                    "ApiKeys": [{ "KeyId": "k", "Key": "kahrad-test-key-0000000000000000000000000001", "Enabled": true }],
                    "SqlServer": { "DataSource": "sql", "Database": "KahradDb", "AuthenticationMode": "IntegratedSecurity" }
                  },
                  "Rodor": {
                    "Enabled": true,
                    "ApiKeys": [{ "KeyId": "r", "Key": "rodor-test-key-00000000000000000000000000002", "Enabled": true }],
                    "SqlServer": { "DataSource": "sql", "Database": "RodorDb", "AuthenticationMode": "IntegratedSecurity" }
                  }
                }
              }
            }
            """;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var configuration = new ConfigurationBuilder().AddJsonStream(stream).Build();
        var options = configuration.GetSection(MahakOptions.SectionName).Get<MahakOptions>()!;
        var resolver = new ApiKeyBrandResolver(Options.Create(options));

        Assert.True(resolver.TryResolve(MahakTestData.KahradKey, out var kahrad));
        Assert.True(resolver.TryResolve(MahakTestData.RodorKey, out var rodor));
        Assert.Equal("KahradDb", kahrad!.Brand.Database);
        Assert.Equal("RodorDb", rodor!.Brand.Database);
        Assert.NotEqual(kahrad.Brand.Database, rodor.Brand.Database);
    }

    [Fact]
    public void Wrong_and_disabled_keys_are_rejected()
    {
        var options = MahakTestData.Options();
        options.Brands["Kahrad"].ApiKeys.Add(new BrandApiKeyOptions
        {
            KeyId = "disabled",
            Key = MahakTestData.DisabledKey,
            Enabled = false
        });
        var resolver = new ApiKeyBrandResolver(Options.Create(options));

        Assert.False(resolver.TryResolve("wrong-key", out _));
        Assert.False(resolver.TryResolve(MahakTestData.DisabledKey, out _));
    }

    [Fact]
    public void Two_enabled_rotation_keys_resolve_to_the_same_brand()
    {
        var options = MahakTestData.Options();
        const string nextKey = "kahrad-next-key-0000000000000000000000000001";
        options.Brands["Kahrad"].ApiKeys.Add(new BrandApiKeyOptions
        {
            KeyId = "kahrad-v2",
            Key = nextKey,
            Enabled = true
        });
        var resolver = new ApiKeyBrandResolver(Options.Create(options));

        Assert.True(resolver.TryResolve(MahakTestData.KahradKey, out var oldResolution));
        Assert.True(resolver.TryResolve(nextKey, out var newResolution));
        Assert.Equal(oldResolution!.Brand.Database, newResolution!.Brand.Database);
    }

    [Fact]
    public void Disabled_brand_key_is_rejected()
    {
        var options = MahakTestData.Options();
        options.Brands["Kahrad"].Enabled = false;
        var resolver = new ApiKeyBrandResolver(Options.Create(options));

        Assert.False(resolver.TryResolve(MahakTestData.KahradKey, out _));
    }

    [Fact]
    public void Duplicate_active_keys_fail_validation_without_echoing_secret()
    {
        var options = MahakTestData.Options();
        options.Brands["Rodor"].ApiKeys[0].Key = MahakTestData.KahradKey;
        var result = new MahakOptionsValidator(new TestHostEnvironment()).Validate(null, options);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Failures!, failure => failure.Contains("same enabled API key"));
        Assert.DoesNotContain(MahakTestData.KahradKey, string.Join(' ', result.Failures!));
    }

    [Fact]
    public void Missing_database_setting_fails_validation()
    {
        var options = MahakTestData.Options();
        options.Brands["Kahrad"].SqlServer.Database = string.Empty;
        var result = new MahakOptionsValidator(new TestHostEnvironment()).Validate(null, options);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Failures!, failure => failure.Contains("DataSource and Database"));
    }

    [Fact]
    public void Production_placeholder_fails_without_disclosing_credentials()
    {
        var options = MahakTestData.Options();
        options.Brands["Kahrad"].SqlServer.Database = "CONFIRM_KAHRAD_DATABASE_NAME";
        var result = new MahakOptionsValidator(new TestHostEnvironment("Production"))
            .Validate(null, options);

        Assert.False(result.Succeeded);
        Assert.Contains(result.Failures!, failure => failure.Contains("placeholder"));
        Assert.DoesNotContain("test-password", string.Join(' ', result.Failures!));
    }

    [Fact]
    public void Connection_factory_uses_only_the_trusted_brand_catalog()
    {
        var brand = new BrandContext(
            "Kahrad", "server", "TrustedCatalog", "IntegratedSecurity",
            null, null, true, false, 5, 20, 30);

        using var connection = new SqlConnectionFactory().Create(brand);
        var settings = new SqlConnectionStringBuilder(connection.ConnectionString);

        Assert.Equal("TrustedCatalog", settings.InitialCatalog);
        Assert.True(settings.IntegratedSecurity);
        Assert.False(settings.PersistSecurityInfo);
    }

    [Fact]
    public void Brand_context_string_representation_never_contains_credentials()
    {
        var brand = new BrandContext(
            "Kahrad", "server", "catalog", "SqlPassword",
            "user", "super-secret", true, false, 5, 20, 30);

        Assert.Equal("Kahrad", brand.ToString());
        Assert.DoesNotContain("super-secret", brand.ToString());
    }
}
