using AutoMapper;
using Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;
using Kahrad.Pricing.Guarantees.Domain.Entities;
using Kahrad.Pricing.Guarantees.Domain.Enums;

namespace Kahrad.Pricing.Guarantees.Application.Mapping;

public sealed class GuaranteeMappingProfile : Profile
{
    public GuaranteeMappingProfile()
    {
        CreateMap<Guarantee, GuaranteeDto>()
            .ForMember(dest => dest.VendorName, opt => opt.MapFrom(src => src.Vendor != null ? src.Vendor.Name : null));

        CreateMap<Guarantee, GuaranteeDetailDto>()
            .IncludeBase<Guarantee, GuaranteeDto>()
            .ForMember(dest => dest.ActiveCertificateCount, opt => opt.MapFrom(src => src.Certificates.Count(c => c.Status == GuaranteeCertificateStatus.Active)))
            .ForMember(dest => dest.TotalCertificateCount, opt => opt.MapFrom(src => src.Certificates.Count));
    }
}
