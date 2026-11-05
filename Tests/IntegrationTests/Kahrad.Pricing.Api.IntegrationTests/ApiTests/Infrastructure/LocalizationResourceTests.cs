using System.Globalization;
using Kahrad.Pricing.Api.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Xunit;

namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Infrastructure;

public sealed class LocalizationResourceTests
{
    [Theory]
    [InlineData("en-US", "Product name is required.")]
    [InlineData("fa-IR", "نام محصول لازم است.")]
    [InlineData("ar-SA", "اسم المنتج مطلوب.")]
    public void Validation_key_is_available_in_each_culture(string culture, string expected)
    {
        using var scope = new CultureScope(culture);
        var messages = CreateLocalizer();
        Assert.Equal(expected, ApiText.Translate(messages, "Product name is required.", "The request is invalid."));
    }

    [Theory]
    [InlineData("en-US", "Product group with ID '42' was not found.")]
    [InlineData("fa-IR", "گروه محصول با شناسه «42» یافت نشد.")]
    [InlineData("ar-SA", "لم يتم العثور على مجموعة المنتجات مع المعرف «42».")]
    public void Identifier_messages_are_formatted_in_each_culture(string culture, string expected)
    {
        using var scope = new CultureScope(culture);
        var messages = CreateLocalizer();
        Assert.Equal(expected, ApiText.Translate(messages,
            "Product group with ID '42' was not found.", "Product group not found"));
    }

    private static IStringLocalizer<ApiMessages> CreateLocalizer()
    {
        var services = new ServiceCollection();
        services.AddLogging(); services.AddLocalization();
        return services.BuildServiceProvider().GetRequiredService<IStringLocalizer<ApiMessages>>();
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo previous = CultureInfo.CurrentCulture;
        private readonly CultureInfo previousUi = CultureInfo.CurrentUICulture;
        public CultureScope(string name)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(name);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(name);
        }
        public void Dispose()
        {
            CultureInfo.CurrentCulture = previous;
            CultureInfo.CurrentUICulture = previousUi;
        }
    }
}
