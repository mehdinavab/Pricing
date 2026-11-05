using Kahrad.Pricing.Products.Domain.Entities;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
namespace Kahrad.Pricing.Products.Application.Abstractions.Services;
public interface ILanguageResolver
{
    CatalogLanguage Resolve(IReadOnlyCollection<Language> languages, IReadOnlyCollection<LanguagePreference> preferences);
}
