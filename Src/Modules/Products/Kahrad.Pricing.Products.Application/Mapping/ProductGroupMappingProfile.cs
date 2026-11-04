using AutoMapper;
using Kahrad.Pricing.Products.Application.DTOs.ProductGroups;
using Kahrad.Pricing.Products.Domain.Entities;

namespace Kahrad.Pricing.Products.Application.Mapping;

public sealed class ProductGroupMappingProfile : Profile
{
    public ProductGroupMappingProfile()
    {
        CreateMap<ProductGroup, ProductGroupDto>();
    }
}
