using Kahrad.Pricing.Products.Domain.Entities;
using Kahrad.Pricing.Products.Infrastructure.Persistence;
using Kahrad.Pricing.Products.Infrastructure.Persistence.Repositories;
using Kahrad.Pricing.Sms.Domain.Entities;
using Kahrad.Pricing.Sms.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
namespace Kahrad.Pricing.Api.IntegrationTests.Setup;

public static class CatalogSetupExtensionMethods
{
    public const string PersianLanguageName = "\u0641\u0627\u0631\u0633\u06cc";
    public const string ArabicLanguageName = "\u0627\u0644\u0639\u0631\u0628\u064a\u0629";
    public const string PersianProductName = "\u0645\u062d\u0635\u0648\u0644 \u0641\u0627\u0631\u0633\u06cc";
    public static async Task PopulateCatalogAndSmsAsync(this KahradPricingApiWebAppFactory factory)
    {
        await using var scope = factory.Services.CreateAsyncScope();
        var products = scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
        products.Languages.AddRange(new Language("fa",PersianLanguageName,true,true),new Language("ar",ArabicLanguageName),new Language("en","English"));
        await products.SaveChangesAsync();
        var sms = scope.ServiceProvider.GetRequiredService<SmsDbContext>();
        var provider = new SmsProviderConfiguration("Kavenegar","Kavenegar","IntegrationTests:UnusedApiKey");
        sms.ProviderConfigurations.Add(provider);await sms.SaveChangesAsync();
        sms.ModuleConfigurations.Add(new SmsModuleConfiguration(provider.Id));await sms.SaveChangesAsync();
    }
    public static async Task<ProductGroup> CreateProductGroupAsync(this KahradPricingApiWebAppFactory factory)
    {
        await using var scope = factory.Services.CreateAsyncScope();var context=scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
        var group = new ProductGroup("Integration group","GROUP-"+Guid.NewGuid().ToString("N"));
        context.ProductGroups.Add(group);await context.SaveChangesAsync();return group;
    }
    public static async Task<Product> CreateProductAsync(this KahradPricingApiWebAppFactory factory,string? name=null,decimal basePrice=100m)
    {
        var group=await factory.CreateProductGroupAsync();
        await using var scope=factory.Services.CreateAsyncScope();var context=scope.ServiceProvider.GetRequiredService<ProductsDbContext>();
        var product=new Product(name??PersianProductName,"PRODUCT-"+Guid.NewGuid().ToString("N"),group.Id,basePrice);
        await new ProductRepository(context).AddAsync(product);return product;
    }
}
