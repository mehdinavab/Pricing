using System.Net;
using System.Net.Http.Json;
using Kahrad.Pricing.Api.IntegrationTests.Setup;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;
using Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;
using Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;
using Kahrad.Pricing.Guarantees.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
namespace Kahrad.Pricing.Api.IntegrationTests.ApiTests.Guarantees;
public sealed class CertificateEndpointTests(KahradPricingApiWebAppFactory factory):ApiTestBase(factory)
{
    [SqlServerFact]
    public async Task Issue_persists_certificate_using_configured_plan_duration()
    {
        var product=await Factory.CreateProductAsync();using var vendor=await Client.PostAsJsonAsync("/api/guarantee-vendors",new CreateVendorRequest{Name="Vendor",Code="VENDOR"});var vendorId=await ReadResultAsync<long>(vendor);
        using var plan=await Client.PostAsJsonAsync("/api/guarantees",new CreateGuaranteeRequest{VendorId=vendorId,Name="One year",Code="YEAR",DurationMonths=12});Assert.Equal(HttpStatusCode.Created,plan.StatusCode);var planId=await ReadResultAsync<long>(plan);
        var purchase=DateTime.UtcNow.Date;using var response=await Client.PostAsJsonAsync("/api/guarantee-certificates",new IssueCertificateRequest{GuaranteeId=planId,ProductId=product.Id,PurchaseDate=purchase,CertificateNumber="TEST-CERT-001"});
        Assert.Equal(HttpStatusCode.Created,response.StatusCode);var certificate=await ReadResultAsync<GuaranteeCertificateDto>(response);Assert.Equal(product.Id,certificate.ProductId);Assert.Equal(purchase.AddMonths(12),certificate.ExpiryDate);
        Assert.True(await ReadDatabaseAsync<GuaranteesDbContext,bool>(db=>db.GuaranteeCertificates.AnyAsync(x=>x.Id==certificate.Id&&x.GuaranteeId==planId)));
    }
    [SqlServerFact]
    public async Task Missing_plan_returns_404()
    {
        var product=await Factory.CreateProductAsync();using var response=await Client.PostAsJsonAsync("/api/guarantee-certificates",new IssueCertificateRequest{GuaranteeId=long.MaxValue,ProductId=product.Id,PurchaseDate=DateTime.UtcNow.Date});Assert.Equal(HttpStatusCode.NotFound,response.StatusCode);
    }
}
