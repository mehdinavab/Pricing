using MAHAK.Integration.Api.Configuration;

namespace MAHAK.Integration.Tests.TestSupport;

internal static class MahakTestData
{
    public const string KahradKey = "kahrad-test-key-0000000000000000000000000001";
    public const string RodorKey = "rodor-test-key-00000000000000000000000000002";
    public const string DisabledKey = "disabled-test-key-00000000000000000000000001";

    public static MahakOptions Options() => new()
    {
        MaximumPageSize = 1000,
        DefaultPageSize = 500,
        Brands = new Dictionary<string, BrandConnectionOptions>(StringComparer.OrdinalIgnoreCase)
        {
            ["Kahrad"] = Brand("Catalog_Kahrad", "kahrad-v1", KahradKey),
            ["Rodor"] = Brand("Catalog_Rodor", "rodor-v1", RodorKey)
        }
    };

    public static BrandConnectionOptions Brand(string database, string keyId, string key) => new()
    {
        Enabled = true,
        ApiKeys =
        [
            new BrandApiKeyOptions { KeyId = keyId, Key = key, Enabled = true }
        ],
        SqlServer = new SqlServerBrandOptions
        {
            DataSource = "test-sql",
            Database = database,
            AuthenticationMode = "SqlPassword",
            UserId = "test-user",
            Password = "test-password",
            Encrypt = true,
            TrustServerCertificate = false,
            ConnectTimeoutSeconds = 5,
            MaxPoolSize = 10
        },
        CommandTimeoutSeconds = 15
    };

    public static Dictionary<string, string?> Configuration() => new()
    {
        ["Mahak:MaximumPageSize"] = "1000",
        ["Mahak:DefaultPageSize"] = "500",
        ["Mahak:Brands:Kahrad:Enabled"] = "true",
        ["Mahak:Brands:Kahrad:ApiKeys:0:KeyId"] = "kahrad-v1",
        ["Mahak:Brands:Kahrad:ApiKeys:0:Key"] = KahradKey,
        ["Mahak:Brands:Kahrad:ApiKeys:0:Enabled"] = "true",
        ["Mahak:Brands:Kahrad:ApiKeys:1:KeyId"] = "kahrad-disabled",
        ["Mahak:Brands:Kahrad:ApiKeys:1:Key"] = DisabledKey,
        ["Mahak:Brands:Kahrad:ApiKeys:1:Enabled"] = "false",
        ["Mahak:Brands:Kahrad:SqlServer:DataSource"] = "test-sql",
        ["Mahak:Brands:Kahrad:SqlServer:Database"] = "Catalog_Kahrad",
        ["Mahak:Brands:Kahrad:SqlServer:AuthenticationMode"] = "SqlPassword",
        ["Mahak:Brands:Kahrad:SqlServer:UserId"] = "test-user",
        ["Mahak:Brands:Kahrad:SqlServer:Password"] = "test-password",
        ["Mahak:Brands:Kahrad:CommandTimeoutSeconds"] = "15",
        ["Mahak:Brands:Rodor:Enabled"] = "true",
        ["Mahak:Brands:Rodor:ApiKeys:0:KeyId"] = "rodor-v1",
        ["Mahak:Brands:Rodor:ApiKeys:0:Key"] = RodorKey,
        ["Mahak:Brands:Rodor:ApiKeys:0:Enabled"] = "true",
        ["Mahak:Brands:Rodor:SqlServer:DataSource"] = "test-sql",
        ["Mahak:Brands:Rodor:SqlServer:Database"] = "Catalog_Rodor",
        ["Mahak:Brands:Rodor:SqlServer:AuthenticationMode"] = "IntegratedSecurity",
        ["Mahak:Brands:Rodor:CommandTimeoutSeconds"] = "15"
    };
}
