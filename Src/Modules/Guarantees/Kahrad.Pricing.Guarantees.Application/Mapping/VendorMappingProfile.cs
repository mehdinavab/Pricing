using AutoMapper;
using Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;
using Kahrad.Pricing.Guarantees.Domain.Entities;

namespace Kahrad.Pricing.Guarantees.Application.Mapping;

public sealed class VendorMappingProfile : Profile
{
    public VendorMappingProfile()
    {
        CreateMap<Vendor, VendorDto>();

        CreateMap<Vendor, VendorDetailDto>()
            .IncludeBase<Vendor, VendorDto>()
            .ForMember(dest => dest.Guarantees, opt => opt.MapFrom(src => src.Guarantees));
    }
}
