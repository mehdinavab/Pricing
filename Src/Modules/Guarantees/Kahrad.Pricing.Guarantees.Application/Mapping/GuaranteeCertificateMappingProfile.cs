using AutoMapper;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;
using Kahrad.Pricing.Guarantees.Domain.Entities;

namespace Kahrad.Pricing.Guarantees.Application.Mapping;

public sealed class GuaranteeCertificateMappingProfile : Profile
{
    public GuaranteeCertificateMappingProfile()
    {
        CreateMap<GuaranteeCertificate, GuaranteeCertificateDto>()
            .ForMember(dest => dest.GuaranteeName, opt => opt.MapFrom(src => src.Guarantee != null ? src.Guarantee.Name : null))
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Guarantee != null && src.Guarantee.Vendor != null ? src.Guarantee.Vendor.Name : null));
    }
}
