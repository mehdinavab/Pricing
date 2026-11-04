using AutoMapper;
using Kahrad.Pricing.Products.Application.DTOs.Features;
using Kahrad.Pricing.Products.Application.DTOs.Gallery;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Kahrad.Pricing.Products.Domain.Entities;

namespace Kahrad.Pricing.Products.Application.Mapping;

public sealed class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group != null ? src.Group.Name : null));

        CreateMap<Product, ProductDetailDto>()
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group != null ? src.Group.Name : null))
            .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group))
            .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Features))
            .ForMember(dest => dest.Gallery, opt => opt.MapFrom(src => src.Gallery));

        CreateMap<ProductFeature, ProductFeatureDto>();

        CreateMap<ProductImage, ProductImageDto>();
    }
}
