using Microsoft.AspNetCore.Localization;
using Microsoft.Net.Http.Headers;

namespace Kahrad.Pricing.Api.Localization;

public sealed class ApiRequestCultureProvider : RequestCultureProvider
{
    public override Task<ProviderCultureResult?> DetermineProviderCultureResult(HttpContext context)
    {
        var header = context.Request.Headers.AcceptLanguage.ToString();
        if (header.Length is > 0 and <= 4096 &&
            StringWithQualityHeaderValue.TryParseList([header], out var values))
        {
            foreach (var value in values.OrderByDescending(x => x.Quality ?? 1))
            {
                if (value.Quality == 0) continue;
                var culture = Match(value.Value.ToString());
                if (culture is not null) return Task.FromResult<ProviderCultureResult?>(new(culture));
            }
        }

        var region = context.Request.Headers["X-Region"].ToString();
        var fromRegion = region.Length <= 32 ? Match(region) : null;
        return Task.FromResult<ProviderCultureResult?>(fromRegion is null ? null : new(fromRegion));
    }

    private static string? Match(string value)
    {
        value = value.Trim();
        if (value.Equals("IR", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("fa", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("fa-", StringComparison.OrdinalIgnoreCase)) return "fa-IR";
        if (value.Equals("SA", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("ar", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("ar-", StringComparison.OrdinalIgnoreCase)) return "ar-SA";
        if (value.Equals("US", StringComparison.OrdinalIgnoreCase) ||
            value.Equals("en", StringComparison.OrdinalIgnoreCase) ||
            value.StartsWith("en-", StringComparison.OrdinalIgnoreCase)) return "en-US";
        return null;
    }
}
