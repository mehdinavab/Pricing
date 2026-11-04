using Kahrad.Pricing.Guarantees.Domain.Entities;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;

namespace Kahrad.Pricing.Guarantees.Infrastructure.Services;

public sealed class GuaranteesSieveProcessor(IOptions<SieveOptions> options)
    : SieveProcessor(options)
{
    protected override SievePropertyMapper MapProperties(
        SievePropertyMapper mapper)
    {
        mapper = base.MapProperties(mapper);

        mapper.Property<Vendor>(v => v.Id).CanSort();
        mapper.Property<Vendor>(v => v.Name).CanFilter().CanSort();
        mapper.Property<Vendor>(v => v.Code).CanFilter().CanSort();
        mapper.Property<Vendor>(v => v.ContactPerson).CanFilter().CanSort();
        mapper.Property<Vendor>(v => v.PhoneNumber).CanFilter().CanSort();
        mapper.Property<Vendor>(v => v.IsActive).CanFilter().CanSort();
        mapper.Property<Vendor>(v => v.CreatedAtUtc).CanSort();

        mapper.Property<Guarantee>(g => g.Id).CanSort();
        mapper.Property<Guarantee>(g => g.VendorId).CanFilter().CanSort();
        mapper.Property<Guarantee>(g => g.Name).CanFilter().CanSort();
        mapper.Property<Guarantee>(g => g.Code).CanFilter().CanSort();
        mapper.Property<Guarantee>(g => g.DurationMonths).CanFilter().CanSort();
        mapper.Property<Guarantee>(g => g.IsActive).CanFilter().CanSort();
        mapper.Property<Guarantee>(g => g.CreatedAtUtc).CanSort();

        mapper.Property<GuaranteeCertificate>(c => c.Id).CanSort();
        mapper.Property<GuaranteeCertificate>(c => c.GuaranteeId).CanFilter().CanSort();
        mapper.Property<GuaranteeCertificate>(c => c.ProductId).CanFilter().CanSort();
        mapper.Property<GuaranteeCertificate>(c => c.CertificateNumber).CanFilter().CanSort();
        mapper.Property<GuaranteeCertificate>(c => c.ProductSerialNumber).CanFilter().CanSort();
        mapper.Property<GuaranteeCertificate>(c => c.StoreId).CanFilter().CanSort();
        mapper.Property<GuaranteeCertificate>(c => c.PurchaseDate).CanFilter().CanSort();
        mapper.Property<GuaranteeCertificate>(c => c.StartDate).CanFilter().CanSort();
        mapper.Property<GuaranteeCertificate>(c => c.ExpiryDate).CanFilter().CanSort();
        mapper.Property<GuaranteeCertificate>(c => c.Status).CanFilter().CanSort();
        mapper.Property<GuaranteeCertificate>(c => c.CustomerName).CanFilter().CanSort();
        mapper.Property<GuaranteeCertificate>(c => c.CustomerPhoneNumber).CanFilter().CanSort();
        mapper.Property<GuaranteeCertificate>(c => c.CreatedAtUtc).CanSort();

        return mapper;
    }
}
