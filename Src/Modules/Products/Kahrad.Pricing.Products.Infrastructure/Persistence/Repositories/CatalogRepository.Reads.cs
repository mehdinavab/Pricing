using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Kahrad.Pricing.Products.Application.DTOs.Features;
using Kahrad.Pricing.Products.Application.DTOs.Gallery;
using Kahrad.Pricing.Products.Application.DTOs.ProductGroups;
using Kahrad.Pricing.Products.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Kahrad.Pricing.Products.Infrastructure.Persistence.Repositories;
public sealed partial class CatalogRepository
{
    private IQueryable<ProductDetailDto> Project(IQueryable<Product> products, CatalogLanguage language) =>
        products.Select(p => new ProductDetailDto
        {
            Id = p.Id,
            PublicId = p.PublicId,
            Code = p.Code,
            GroupId = p.GroupId,
            BrandId = p.BrandId,
            BasePrice = p.BasePrice,
            BeautyFactorPercentage = p.BeautyFactorPercentage,
            CommercialFactorPercentage = p.CommercialFactorPercentage,
            SalesFactorPercentage = p.SalesFactorPercentage,
            IsActive = p.IsActive,
            CreatedAtUtc = p.CreatedAtUtc,
            UpdatedAtUtc = p.UpdatedAtUtc,
            Language = language.Code,
            Name = p.Translations.Where(t => t.LanguageId == language.Id).Select(t => t.Name).FirstOrDefault()
                ?? p.Translations.Where(t => t.LanguageId == language.DefaultId).Select(t => t.Name).FirstOrDefault() ?? p.Name,
            Description = p.Translations.Where(t => t.LanguageId == language.Id).Select(t => t.LocalizedDescription).FirstOrDefault()
                ?? p.Translations.Where(t => t.LanguageId == language.DefaultId).Select(t => t.LocalizedDescription).FirstOrDefault() ?? p.Description,
            GroupName = p.Group.Name,
            Group = new ProductGroupDto { Id = p.Group.Id, PublicId = p.Group.PublicId, Name = p.Group.Name,
                Code = p.Group.Code, PriceEffectPercentage = p.Group.PriceEffectPercentage, Description = p.Group.Description,
                IsActive = p.Group.IsActive, CreatedAtUtc = p.Group.CreatedAtUtc, UpdatedAtUtc = p.Group.UpdatedAtUtc },
            Features = p.Features.OrderBy(f => f.DisplayOrder).ThenBy(f => f.Id).Select(f => new ProductFeatureDto
            {
                Id = f.Id, PublicId = f.PublicId, ProductId = f.ProductId, FeatureId = f.FeatureId,
                Name = context.FeatureTranslations.Where(t => t.FeatureId == f.FeatureId && t.LanguageId == language.Id).Select(t => t.Name).FirstOrDefault()
                    ?? context.FeatureTranslations.Where(t => t.FeatureId == f.FeatureId && t.LanguageId == language.DefaultId).Select(t => t.Name).FirstOrDefault() ?? f.Name,
                Value = f.Value, HasPriceEffect = f.HasPriceEffect, PriceEffectPercentage = f.PriceEffectPercentage,
                PriceEffectAmount = f.PriceEffectAmount, DisplayOrder = f.DisplayOrder, Description = f.Description,
                CreatedAtUtc = f.CreatedAtUtc, UpdatedAtUtc = f.UpdatedAtUtc
            }).ToList(),
            Gallery = p.Gallery.OrderBy(g => g.DisplayOrder).ThenBy(g => g.Id).Select(g => new ProductImageDto
            {
                Id = g.Id, PublicId = g.PublicId, ProductId = g.ProductId, ImageUrl = g.ImageUrl, AltText = g.AltText,
                DisplayOrder = g.DisplayOrder, IsPrimary = g.IsPrimary, Description = g.Description,
                CreatedAtUtc = g.CreatedAtUtc, UpdatedAtUtc = g.UpdatedAtUtc
            }).ToList(),
            Specifications = (from v in context.ProductSpecifications
                join s in context.Specifications on v.SpecificationId equals s.Id
                where v.ProductId == p.Id orderby v.DisplayOrder, v.Id
                select new ProductSpecificationDto
                {
                    Id = v.Id, SpecificationId = s.Id, Unit = s.Unit, ValueType = s.ValueType,
                    Name = context.SpecificationTranslations.Where(t => t.SpecificationId == s.Id && t.LanguageId == language.Id).Select(t => t.Name).FirstOrDefault()
                        ?? context.SpecificationTranslations.Where(t => t.SpecificationId == s.Id && t.LanguageId == language.DefaultId).Select(t => t.Name).FirstOrDefault() ?? s.Name,
                    TextValue = context.ProductSpecificationTranslations.Where(t => t.ProductSpecificationId == v.Id && t.LanguageId == language.Id).Select(t => t.Value).FirstOrDefault()
                        ?? context.ProductSpecificationTranslations.Where(t => t.ProductSpecificationId == v.Id && t.LanguageId == language.DefaultId).Select(t => t.Value).FirstOrDefault() ?? v.TextValue,
                    NumberValue = v.NumberValue, BooleanValue = v.BooleanValue, DateValue = v.DateValue, CodeValue = v.CodeValue
                }).ToList()
        });
    public async Task<BaseResponsePaginated<ProductDetailDto>> SearchAsync(CatalogSearchRequest request, CatalogLanguage language, CancellationToken ct)
    {
        var products = context.Products.AsNoTracking().Where(x => x.IsActive && !x.Group.IsDeleted);
        if (request.BrandId.HasValue) products = products.Where(x => x.BrandId == request.BrandId);
        if (request.GroupId.HasValue) products = products.Where(x => x.GroupId == request.GroupId);
        if (!string.IsNullOrWhiteSpace(request.Code)) products = products.Where(x => x.Code.Contains(request.Code));
        if (!string.IsNullOrWhiteSpace(request.Name)) products = products.Where(p =>
            (p.Translations.Where(t => t.LanguageId == language.Id).Select(t => t.Name).FirstOrDefault()
             ?? p.Translations.Where(t => t.LanguageId == language.DefaultId).Select(t => t.Name).FirstOrDefault() ?? p.Name).Contains(request.Name));
        var total = await products.CountAsync(ct);
        var ordered = request.SortByName ? products.OrderBy(p =>
            p.Translations.Where(t => t.LanguageId == language.Id).Select(t => t.Name).FirstOrDefault()
             ?? p.Translations.Where(t => t.LanguageId == language.DefaultId).Select(t => t.Name).FirstOrDefault() ?? p.Name).ThenBy(x => x.Id)
            : products.OrderBy(x => x.Id);
        var page = ordered.Skip(checked((request.Page - 1) * request.PageSize)).Take(request.PageSize);
        await DiagnoseFallbackAsync(page, language, ct);
        var result = await Project(page, language).AsSplitQuery().ToListAsync(ct);
        return BaseResponsePaginated<ProductDetailDto>.Success(result, request.Page, request.PageSize, total);
    }
    public async Task<ProductDetailDto?> GetDetailsAsync(long id, CatalogLanguage language, CancellationToken ct)
    {
        var query = context.Products.AsNoTracking().Where(x => x.Id == id && x.IsActive && !x.Group.IsDeleted);
        await DiagnoseFallbackAsync(query, language, ct);
        return await Project(query, language).AsSplitQuery().SingleOrDefaultAsync(ct);
    }
    private async Task DiagnoseFallbackAsync(IQueryable<Product> query, CatalogLanguage language, CancellationToken ct)
    {
        if (await query.AnyAsync(p => !p.Translations.Any(t => t.LanguageId == language.DefaultId), ct))
            logger.LogWarning("Catalog used legacy content because a default-language translation is missing. DefaultLanguage={Language}", language.DefaultCode);
    }
}
