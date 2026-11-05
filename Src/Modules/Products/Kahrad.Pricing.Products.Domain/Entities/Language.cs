using Kahrad.Pricing.Products.Domain.Exceptions;
using System.Globalization;
using System.Text.RegularExpressions;
using Kahrad.Pricing.BuildingBlocks.Domain.Common;
namespace Kahrad.Pricing.Products.Domain.Entities;
public sealed class Language : BaseEntity
{
    public string Code { get; private set; } = null!;
    public string Name { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public bool IsDefault { get; private set; }
    private Language() { }
    public Language(string code, string name, bool isActive = true, bool isDefault = false)
    {
        Code = NormalizeCode(code);
        Configure(name, isActive, isDefault);
    }
    public static string NormalizeCode(string code)
    {
        if (string.IsNullOrWhiteSpace(code)) throw new CatalogRuleException("A nonblank value is required.");
        code = code.Trim();
        if (code.Length > 16 || !Regex.IsMatch(code, "^[a-zA-Z]{2,8}(-[a-zA-Z0-9]{1,8})*$"))
            throw new CatalogRuleException("Invalid BCP-47 language code.", nameof(code));
        try { return CultureInfo.GetCultureInfo(code).Name; }
        catch (CultureNotFoundException) { throw new CatalogRuleException("Invalid BCP-47 language code."); }
    }
    public void ReplaceDefault(Language previous, bool allActiveProductsHaveTranslation)
    {
        if (!allActiveProductsHaveTranslation) throw new CatalogRuleException("Every active product needs a translation in the proposed default language.");
        if (!IsActive || IsDeleted) throw new CatalogRuleException("The new default must be active.");
        if (!previous.IsDefault) throw new CatalogRuleException("Previous language is not default.");
        previous.IsDefault = false;
        IsDefault = true;
    }
    public void Configure(string name, bool isActive, bool isDefault)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new CatalogRuleException("A nonblank value is required.");
        if (name.Trim().Length > 100) throw new CatalogRuleException("Language name is too long.");
        if (IsDefault && !isDefault) throw new CatalogRuleException("Switch to another active default language instead.");
        if (isDefault && !isActive) throw new CatalogRuleException("The default language must remain active.");
        Name = name.Trim(); IsActive = isActive; IsDefault = isDefault;
    }
}
