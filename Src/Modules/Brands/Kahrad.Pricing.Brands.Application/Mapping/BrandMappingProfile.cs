using AutoMapper;
using Kahrad.Pricing.Brands.Application.DTOs.Brands;
using Kahrad.Pricing.Brands.Domain.Entities;

namespace Kahrad.Pricing.Brands.Application.Mapping;

public sealed class BrandMappingProfile : Profile
{
    public BrandMappingProfile()
    {
        CreateMap<Brand, BrandDto>();

        CreateMap<Brand, BrandDetailDto>()
            .IncludeBase<Brand, BrandDto>()
            .ForMember(dest => dest.Configs, opt => opt.MapFrom(src => src.Configs))
            .ForMember(dest => dest.StoreCount, opt => opt.MapFrom(src => src.Stores.Count));
    }
}
