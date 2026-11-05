using FluentValidation;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Brands.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Identity.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.Abstractions.Services;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Kahrad.Pricing.Sms.Application.Abstractions.Services;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Infrastructure;
public sealed class DependencyInjectionTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    [SqlServerFact]
    public void Implemented_module_services_and_validators_resolve()
    {
        using var scope=Factory.Services.CreateScope();var services=scope.ServiceProvider;
        foreach(var type in new[]{typeof(IAuthenticationService),typeof(IUserService),typeof(IRoleService),typeof(IPermissionService),typeof(IAuditService),
            typeof(IProductService),typeof(IProductGroupService),typeof(ICatalogService),typeof(IBrandService),typeof(IStoreService),
            typeof(IVendorService),typeof(IGuaranteeService),typeof(IGuaranteeCertificateService),typeof(ISmsService),typeof(ISmsOutboxProcessor)})
            Assert.NotNull(services.GetRequiredService(type));
        Assert.NotNull(services.GetRequiredService<IValidator<CreateProductRequest>>());
    }
    [SqlServerFact]
    public void Factory_always_uses_its_generated_database()
    {
        var settings=new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(Factory.ConnectionString);
        Assert.StartsWith("KahradApiTests_",settings.InitialCatalog);Assert.NotEqual("KahradPricing",settings.InitialCatalog);
    }
}
