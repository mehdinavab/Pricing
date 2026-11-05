using Kahrad.Pricing.Products.Domain.Exceptions;
using Kahrad.Pricing.BuildingBlocks.Domain.Common;
namespace Kahrad.Pricing.Products.Domain.Entities;
public sealed class FeatureTranslation : BaseEntity
{
    public long FeatureId { get; private set; }
    public long LanguageId { get; private set; }
    public string Name { get; private set; } = null!;
    private FeatureTranslation() { }
    public FeatureTranslation(long entityId, long languageId, string text)
    {
        if (entityId <= 0 || languageId <= 0) throw new CatalogRuleException("Entity and language IDs must be positive.");
        FeatureId = entityId; LanguageId = languageId;
        Update(text);
    }
    public void Update(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) throw new CatalogRuleException("A nonblank value is required.");
        if (text.Trim().Length > 150) throw new CatalogRuleException("Translation is too long.");
        Name = text.Trim();
    }
}
