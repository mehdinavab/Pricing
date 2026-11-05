using Kahrad.Pricing.Products.Domain.Entities;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
namespace Kahrad.Pricing.Products.Application.Services;
public sealed class LanguageResolver : ILanguageResolver
{
    public CatalogLanguage Resolve(IReadOnlyCollection<Language> languages, IReadOnlyCollection<LanguagePreference> preferences)
    {
        var active = languages.Where(x => x.IsActive && !x.IsDeleted).ToArray();
        var fallback = active.SingleOrDefault(x => x.IsDefault)
            ?? throw new InvalidOperationException("Catalog requires exactly one active default language.");
        bool Allowed(string code) => !preferences.Any(x => x.Quality == 0 &&
            (x.Code.Equals(code, StringComparison.OrdinalIgnoreCase) ||
             (x.Code == "*" && !preferences.Any(p => p.Quality > 0 && p.Code.Equals(code, StringComparison.OrdinalIgnoreCase))) ||
             (code.StartsWith(x.Code + "-", StringComparison.OrdinalIgnoreCase) &&
              !preferences.Any(p => p.Quality > 0 && p.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))));
        Language? selected = null;
        foreach (var preference in preferences.Where(x => x.Quality > 0 && x.Quality <= 1).OrderByDescending(x => x.Quality).ThenBy(x => x.Order))
        {
            if (preference.Code == "*")
            {
                selected = Allowed(fallback.Code) ? fallback : active.OrderBy(x => x.Code).FirstOrDefault(x => Allowed(x.Code));
            }
            else
            {
                var code = preference.Code;
                while (!string.IsNullOrWhiteSpace(code))
                {
                    selected = active.FirstOrDefault(x => x.Code.Equals(code, StringComparison.OrdinalIgnoreCase) && Allowed(x.Code));
                    if (selected is not null) break;
                    var separator = code.LastIndexOf('-');
                    code = separator > 0 ? code[..separator] : "";
                }
            }
            if (selected is not null) break;
        }
        selected ??= fallback; // Deliberately serve the configured default, rather than 406, when all acceptable locales fail.
        return new CatalogLanguage(selected.Id, selected.Code, fallback.Id, fallback.Code);
    }
}
