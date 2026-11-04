using AutoMapper;
using Kahrad.Pricing.Brands.Application.DTOs.Stores;
using Kahrad.Pricing.Brands.Domain.Entities;

namespace Kahrad.Pricing.Brands.Application.Mapping;

public sealed class StoreMappingProfile : Profile
{
    public StoreMappingProfile()
    {
        CreateMap<Store, StoreDto>()
            .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null));
    }
}
