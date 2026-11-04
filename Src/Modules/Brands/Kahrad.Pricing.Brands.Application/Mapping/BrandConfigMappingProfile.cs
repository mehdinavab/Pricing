using AutoMapper;
using Kahrad.Pricing.Brands.Application.DTOs.BrandConfigs;
using Kahrad.Pricing.Brands.Domain.Entities;

namespace Kahrad.Pricing.Brands.Application.Mapping;

public sealed class BrandConfigMappingProfile : Profile
{
    public BrandConfigMappingProfile()
    {
        CreateMap<BrandConfig, BrandConfigDto>();
    }
}
