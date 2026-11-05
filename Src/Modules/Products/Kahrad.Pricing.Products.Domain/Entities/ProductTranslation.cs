using Kahrad.Pricing.Products.Domain.Exceptions;
using Kahrad.Pricing.BuildingBlocks.Domain.Common;
namespace Kahrad.Pricing.Products.Domain.Entities;
public sealed class ProductTranslation : BaseEntity
{
    public long ProductId { get; private set; }
    public long LanguageId { get; private set; }
    public string Name { get; private set; } = null!;
    public string? LocalizedDescription { get; private set; }
    private ProductTranslation() { }
    public ProductTranslation(long entityId, long languageId, string text, string? description = null)
    {
        if (entityId <= 0 || languageId <= 0) throw new CatalogRuleException("Entity and language IDs must be positive.");
        ProductId = entityId; LanguageId = languageId;
        Update(text, description);
    }
    public void Update(string text, string? description = null)
    {
        if (string.IsNullOrWhiteSpace(text)) throw new CatalogRuleException("A nonblank value is required.");
        if (text.Trim().Length > 250) throw new CatalogRuleException("Translation is too long.");
        Name = text.Trim();
        if (description?.Length > 1000) throw new CatalogRuleException("Description is too long.");
        LocalizedDescription = description?.Trim();
    }
}
