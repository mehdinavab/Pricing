using Kahrad.Pricing.Products.Application.DTOs.Features;
using Kahrad.Pricing.Products.Application.DTOs.Gallery;
using Kahrad.Pricing.Products.Application.DTOs.ProductGroups;

namespace Kahrad.Pricing.Products.Application.DTOs.Products;

public class ProductDetailDto : ProductDto
{
    public ProductGroupDto? Group { get; init; }
    public List<ProductFeatureDto> Features { get; init; } = [];
    public List<ProductImageDto> Gallery { get; init; } = [];
}
