using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace MAHAK.Integration.Api.Configuration;

public sealed class MahakOptionsValidator(IHostEnvironment environment)
    : IValidateOptions<MahakOptions>
{
    private static readonly string[] Placeholders = ["REPLACE_", "YOUR_", "CONFIRM_"];

    public ValidateOptionsResult Validate(string? name, MahakOptions options)
    {
        var errors = new List<string>();
        if (options.MaximumPageSize is < 1 or > 5000)
            errors.Add("Mahak:MaximumPageSize must be between 1 and 5000.");
        if (options.DefaultPageSize < 1 || options.DefaultPageSize > options.MaximumPageSize)
            errors.Add("Mahak:DefaultPageSize must be positive and no greater than MaximumPageSize.");

        var activeKeyHashes = new HashSet<string>(StringComparer.Ordinal);
        var enabledBrands = 0;
        foreach (var (brandCode, brand) in options.Brands)
        {
            if (string.IsNullOrWhiteSpace(brandCode))
                errors.Add("Mahak brand codes cannot be blank.");

            var keyIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var key in brand.ApiKeys)
            {
                if (string.IsNullOrWhiteSpace(key.KeyId) || !keyIds.Add(key.KeyId))
                    errors.Add($"Brand '{brandCode}' has a blank or duplicate API KeyId.");
            }

            if (!brand.Enabled)
                continue;

            enabledBrands++;
            var enabledKeys = brand.ApiKeys.Where(x => x.Enabled).ToArray();
            if (enabledKeys.Length == 0)
                errors.Add($"Enabled brand '{brandCode}' requires at least one enabled API key.");

            foreach (var key in enabledKeys)
            {
                if (key.Key.Length < 32)
                    errors.Add($"Enabled key '{key.KeyId}' for brand '{brandCode}' must contain at least 32 characters.");
                var digest = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(key.Key)));
                if (!activeKeyHashes.Add(digest))
                    errors.Add("The same enabled API key cannot be assigned to multiple brands or key entries.");
                if (environment.IsProduction() && ContainsPlaceholder(key.Key))
                    errors.Add($"Enabled key '{key.KeyId}' for brand '{brandCode}' still contains a placeholder.");
            }

            ValidateSql(brandCode, brand, errors);
        }

        if (environment.IsProduction() && enabledBrands == 0)
            errors.Add("Production requires at least one enabled MAHAK brand.");

        return errors.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(errors);
    }

    private void ValidateSql(
        string brandCode,
        BrandConnectionOptions brand,
        ICollection<string> errors)
    {
        var sql = brand.SqlServer;
        if (string.IsNullOrWhiteSpace(sql.DataSource) || string.IsNullOrWhiteSpace(sql.Database))
            errors.Add($"Enabled brand '{brandCode}' requires SQL DataSource and Database settings.");
        if (brand.CommandTimeoutSeconds is < 1 or > 300)
            errors.Add($"Brand '{brandCode}' command timeout must be between 1 and 300 seconds.");
        if (sql.ConnectTimeoutSeconds is < 1 or > 60)
            errors.Add($"Brand '{brandCode}' connection timeout must be between 1 and 60 seconds.");
        if (sql.MaxPoolSize is < 1 or > 500)
            errors.Add($"Brand '{brandCode}' maximum pool size must be between 1 and 500.");

        var integrated = sql.AuthenticationMode.Equals(
            "IntegratedSecurity",
            StringComparison.OrdinalIgnoreCase);
        var sqlPassword = sql.AuthenticationMode.Equals(
            "SqlPassword",
            StringComparison.OrdinalIgnoreCase);
        if (!integrated && !sqlPassword)
            errors.Add($"Brand '{brandCode}' AuthenticationMode must be SqlPassword or IntegratedSecurity.");
        if (sqlPassword && (string.IsNullOrWhiteSpace(sql.UserId) || string.IsNullOrWhiteSpace(sql.Password)))
            errors.Add($"Brand '{brandCode}' requires SQL UserId and Password for SqlPassword authentication.");

        if (environment.IsProduction() &&
            (ContainsPlaceholder(sql.DataSource) || ContainsPlaceholder(sql.Database) ||
             ContainsPlaceholder(sql.UserId) || ContainsPlaceholder(sql.Password)))
        {
            errors.Add($"Enabled brand '{brandCode}' SQL configuration still contains a placeholder.");
        }
    }

    private static bool ContainsPlaceholder(string? value) =>
        value is not null && Placeholders.Any(
            marker => value.Contains(marker, StringComparison.OrdinalIgnoreCase));
}
