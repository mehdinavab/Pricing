using System.Security.Cryptography;
using System.Text;
using MAHAK.Integration.Api.Configuration;
using Microsoft.Extensions.Options;

namespace MAHAK.Integration.Api.Security;

public interface IApiKeyBrandResolver
{
    bool TryResolve(string apiKey, out ApiKeyResolution? resolution);
}

public interface IBrandCatalog
{
    IReadOnlyCollection<BrandContext> EnabledBrands { get; }
}

public sealed class ApiKeyBrandResolver : IApiKeyBrandResolver, IBrandCatalog
{
    private readonly Entry[] entries;
    public IReadOnlyCollection<BrandContext> EnabledBrands { get; }

    public ApiKeyBrandResolver(IOptions<MahakOptions> options)
    {
        EnabledBrands = options.Value.Brands
            .Where(x => x.Value.Enabled)
            .Select(pair => ToContext(pair.Key, pair.Value))
            .ToArray();
        entries = options.Value.Brands
            .Where(x => x.Value.Enabled)
            .SelectMany(pair => pair.Value.ApiKeys
                .Where(key => key.Enabled)
                .Select(key => new Entry(
                    SHA256.HashData(Encoding.UTF8.GetBytes(key.Key)),
                    key.KeyId,
                    ToContext(pair.Key, pair.Value))))
            .ToArray();
    }

    public bool TryResolve(string apiKey, out ApiKeyResolution? resolution)
    {
        var candidateHash = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));
        Entry? match = null;
        foreach (var entry in entries)
        {
            if (CryptographicOperations.FixedTimeEquals(candidateHash, entry.KeyHash))
                match = entry;
        }

        resolution = match is null ? null : new ApiKeyResolution(match.Brand, match.KeyId);
        return resolution is not null;
    }

    private static BrandContext ToContext(string code, BrandConnectionOptions brand) =>
        new(
            code,
            brand.SqlServer.DataSource,
            brand.SqlServer.Database,
            brand.SqlServer.AuthenticationMode,
            brand.SqlServer.UserId,
            brand.SqlServer.Password,
            brand.SqlServer.Encrypt,
            brand.SqlServer.TrustServerCertificate,
            brand.SqlServer.ConnectTimeoutSeconds,
            brand.SqlServer.MaxPoolSize,
            brand.CommandTimeoutSeconds);

    private sealed record Entry(byte[] KeyHash, string KeyId, BrandContext Brand);
}
