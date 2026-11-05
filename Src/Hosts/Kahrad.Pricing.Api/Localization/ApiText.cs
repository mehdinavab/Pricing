using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;

namespace Kahrad.Pricing.Api.Localization;

public static class ApiText
{
    private static readonly Regex IdentifiedMessage = new(
        "^(?<prefix>A |Another )?(?<entity>SMS message|Product feature|Product group|Product image|Product|Guarantee certificate|Guarantee|Vendor|Brand|Store|brand|store|vendor|certificate|guarantee|product group|product) with (the )?(?<kind>Public ID|ID|code|number) '(?<value>[^']*)'(?<owner> for (Vendor|Brand) '[^']*')? (?<ending>was not found|already exists)(?<ownerAfter> for (Vendor|Brand) '[^']*')?\\.$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    public static string Translate(IStringLocalizer<ApiMessages> messages, string? source, string fallback)
    {
        if (string.IsNullOrWhiteSpace(source)) return messages[fallback];
        var translated = messages[source];
        if (!translated.ResourceNotFound) return translated.Value;
        if (source.Any(c => c is >= '\u0600' and <= '\u06ff')) return source;

        var match = IdentifiedMessage.Match(source);
        if (match.Success)
        {
            var entity = messages[match.Groups["entity"].Value.ToLowerInvariant()].Value;
            if (char.IsUpper(match.Groups["entity"].Value[0]))
                entity = char.ToUpperInvariant(entity[0]) + entity[1..];
            var kind = messages[match.Groups["kind"].Value].Value;
            var key = match.Groups["ending"].Value == "was not found"
                ? "Identified record not found"
                : "Identified record already exists";
            var owner = match.Groups["owner"].Success
                ? match.Groups["owner"].Value : match.Groups["ownerAfter"].Value;
            if (owner.Length > 0)
            {
                var ownerParts = owner.Trim().Split(' ', 3);
                key += " for owner";
                return string.Format(System.Globalization.CultureInfo.CurrentUICulture,
                    messages[key].Value, entity, kind, match.Groups["value"].Value,
                    messages[ownerParts[1].ToLowerInvariant()].Value, ownerParts[2].Trim('\''));
            }
            return string.Format(System.Globalization.CultureInfo.CurrentUICulture,
                messages[key].Value, entity, kind, match.Groups["value"].Value);
        }

        return messages[fallback];
    }
}
