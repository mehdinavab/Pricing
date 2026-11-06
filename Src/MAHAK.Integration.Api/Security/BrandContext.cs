namespace MAHAK.Integration.Api.Security;

public sealed record BrandContext(
    string BrandCode,
    string DataSource,
    string Database,
    string AuthenticationMode,
    string? UserId,
    string? Password,
    bool Encrypt,
    bool TrustServerCertificate,
    int ConnectTimeoutSeconds,
    int MaxPoolSize,
    int CommandTimeoutSeconds)
{
    public override string ToString() => BrandCode;
}

public sealed record ApiKeyResolution(BrandContext Brand, string KeyId);
