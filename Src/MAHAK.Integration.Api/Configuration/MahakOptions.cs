namespace MAHAK.Integration.Api.Configuration;

public sealed class MahakOptions
{
    public const string SectionName = "Mahak";

    public int MaximumPageSize { get; set; } = 1000;
    public int DefaultPageSize { get; set; } = 500;
    public Dictionary<string, BrandConnectionOptions> Brands { get; set; } =
        new(StringComparer.OrdinalIgnoreCase);
}

public sealed class BrandConnectionOptions
{
    public bool Enabled { get; set; }
    public List<BrandApiKeyOptions> ApiKeys { get; set; } = [];
    public SqlServerBrandOptions SqlServer { get; set; } = new();
    public int CommandTimeoutSeconds { get; set; } = 30;
}

public sealed class BrandApiKeyOptions
{
    public string KeyId { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public bool Enabled { get; set; }
}

public sealed class SqlServerBrandOptions
{
    public string DataSource { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string AuthenticationMode { get; set; } = "SqlPassword";
    public string? UserId { get; set; }
    public string? Password { get; set; }
    public bool Encrypt { get; set; } = true;
    public bool TrustServerCertificate { get; set; }
    public int ConnectTimeoutSeconds { get; set; } = 5;
    public int MaxPoolSize { get; set; } = 30;
}
