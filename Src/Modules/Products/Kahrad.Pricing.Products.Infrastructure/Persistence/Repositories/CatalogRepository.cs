using System.Data;
using Kahrad.Pricing.BuildingBlocks.Domain.Common;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Products.Application.DTOs.Catalog;
using Kahrad.Pricing.Products.Application.DTOs.Products;
using Kahrad.Pricing.Products.Application.DTOs.Features;
using Kahrad.Pricing.Products.Application.DTOs.Gallery;
using Kahrad.Pricing.Products.Application.DTOs.ProductGroups;
using Kahrad.Pricing.Products.Application.Exceptions;
using Kahrad.Pricing.Products.Domain.Entities;
using Kahrad.Pricing.Products.Domain.Enums;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace Kahrad.Pricing.Products.Infrastructure.Persistence.Repositories;
public sealed partial class CatalogRepository(ProductsDbContext context, ILogger<CatalogRepository> logger) : ICatalogRepository
{
    public Task<bool> HasDefaultTranslationAsync(long productId, CancellationToken ct) =>
        context.ProductTranslations.AnyAsync(t => t.ProductId == productId && context.Languages.Any(l => l.Id == t.LanguageId && l.IsActive && l.IsDefault), ct);
    public Task<Feature?> GetFeatureAsync(long featureId, CancellationToken ct) => context.Features.AsNoTracking().SingleOrDefaultAsync(x => x.Id == featureId, ct);
    public async Task<IReadOnlyCollection<Language>> GetLanguagesAsync(CancellationToken ct) =>
        await context.Languages.AsNoTracking().OrderBy(x => x.Code).ToListAsync(ct);

    private async Task<Language> DefaultAsync(CancellationToken ct) =>
        await context.Languages.SingleOrDefaultAsync(x => x.IsDefault && x.IsActive, ct)
        ?? throw new ProductOperationException("An active default catalog language is required; apply catalog migrations.");

    private static void Concurrency(BaseEntity entity, byte[]? version)
    {
        if (version is null || !entity.RowVersion.SequenceEqual(version))
            throw new CatalogConflictException("The record changed or its row version is missing. Reload it before editing.");
    }
    private async Task SaveAsync(CancellationToken ct)
    {
        try { await context.SaveChangesAsync(ct); }
        catch (DbUpdateConcurrencyException) { throw new CatalogConflictException("The catalog record was changed by another editor."); }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        { throw new CatalogConflictException("A catalog record with this language, code or assignment already exists."); }
    }
    public async Task UpsertLanguageAsync(LanguageRequest request, Guid? userId, CancellationToken ct)
    {
        var code = Language.NormalizeCode(request.Code);
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var language = await context.Languages.SingleOrDefaultAsync(x => x.Code == code, ct);
        var isNew = language is null;
        if (language is not null) Concurrency(language, request.RowVersion);
        language ??= new Language(code, request.Name, request.IsActive, false);
        var previous = await context.Languages.SingleAsync(x => x.IsDefault && x.IsActive, ct);
        if (request.IsDefault && previous.Id != language.Id)
        {
            var allTranslated = !await context.Products.AnyAsync(p => p.IsActive && !context.ProductTranslations.Any(t => t.ProductId == p.Id && t.LanguageId == language.Id), ct);
            if (!isNew) context.Entry(language).State = EntityState.Detached;
            language.Configure(request.Name, request.IsActive, false);
            language.ReplaceDefault(previous, allTranslated);
            previous.SetUpdated(userId, DateTime.UtcNow);
            context.Languages.Update(previous);
            await SaveAsync(ct); // Clear previous default first, inside this transaction, to respect the unique filtered index.
        }
        else language.Configure(request.Name, request.IsActive, request.IsDefault);
        if (isNew) { language.SetCreated(userId, DateTime.UtcNow); context.Languages.Add(language); }
        else { language.SetUpdated(userId, DateTime.UtcNow); context.Languages.Update(language); }
        await SaveAsync(ct); await transaction.CommitAsync(ct);
    }
    public async Task UpsertTranslationAsync(TranslationTarget target, TranslationRequest request, Guid? userId, CancellationToken ct)
    {
        var code = Language.NormalizeCode(request.LanguageCode);
        var language = await context.Languages.SingleOrDefaultAsync(x => x.Code == code, ct)
            ?? throw new ProductOperationException("Language does not exist.");
        BaseEntity translation;
        switch (target)
        {
            case TranslationTarget.Product:
                var product = await context.Products.SingleOrDefaultAsync(x => x.Id == request.EntityId, ct)
                    ?? throw new ProductNotFoundException(request.EntityId);
                var pt = await context.ProductTranslations.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.ProductId == product.Id && x.LanguageId == language.Id, ct);
                if (pt is null) { pt = new ProductTranslation(product.Id, language.Id, request.Text, request.Description); context.ProductTranslations.Add(pt); }
                else { Concurrency(pt, request.RowVersion); pt.Update(request.Text, request.Description); pt.SetRestored(); }
                if (language.IsDefault) { product.SetDefaultContent(pt.Name, pt.LocalizedDescription); product.SetUpdated(userId, DateTime.UtcNow); }
                translation = pt; break;
            case TranslationTarget.Feature:
                var feature = await context.Features.SingleOrDefaultAsync(x => x.Id == request.EntityId, ct)
                    ?? throw new ProductOperationException("Shared feature does not exist.");
                var ft = await context.FeatureTranslations.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.FeatureId == feature.Id && x.LanguageId == language.Id, ct);
                if (ft is null) { ft = new FeatureTranslation(feature.Id, language.Id, request.Text); context.FeatureTranslations.Add(ft); }
                else { Concurrency(ft, request.RowVersion); ft.Update(request.Text); ft.SetRestored(); }
                if (language.IsDefault) feature.SetDefaultName(ft.Name);
                translation = ft; break;
            case TranslationTarget.Specification:
                var specification = await context.Specifications.SingleOrDefaultAsync(x => x.Id == request.EntityId, ct)
                    ?? throw new ProductOperationException("Specification does not exist.");
                var st = await context.SpecificationTranslations.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.SpecificationId == specification.Id && x.LanguageId == language.Id, ct);
                if (st is null) { st = new SpecificationTranslation(specification.Id, language.Id, request.Text); context.SpecificationTranslations.Add(st); }
                else { Concurrency(st, request.RowVersion); st.Update(request.Text); st.SetRestored(); }
                if (language.IsDefault) specification.SetDefaultName(st.Name);
                translation = st; break;
            case TranslationTarget.ProductSpecification:
                var assignment = await context.ProductSpecifications.SingleOrDefaultAsync(x => x.Id == request.EntityId, ct)
                    ?? throw new ProductOperationException("Specification assignment does not exist.");
                var definition = await context.Specifications.SingleAsync(x => x.Id == assignment.SpecificationId, ct);
                assignment.EnsureTextual(definition);
                var vt = await context.ProductSpecificationTranslations.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.ProductSpecificationId == assignment.Id && x.LanguageId == language.Id, ct);
                if (vt is null) { vt = new ProductSpecificationTranslation(assignment.Id, language.Id, request.Text); context.ProductSpecificationTranslations.Add(vt); }
                else { Concurrency(vt, request.RowVersion); vt.Update(request.Text); vt.SetRestored(); }
                if (language.IsDefault) assignment.SetValue(definition.ValueType, vt.Value, null, null, null, null, assignment.DisplayOrder);
                translation = vt; break;
            default: throw new ProductOperationException("Invalid translation target.");
        }
        if (translation.Id == 0) translation.SetCreated(userId, DateTime.UtcNow);
        else translation.SetUpdated(userId, DateTime.UtcNow);
        await SaveAsync(ct);
    }
    public async Task<IReadOnlyCollection<TranslationDto>> GetTranslationsAsync(TranslationTarget target, long id, CancellationToken ct)
    {
        return target switch
        {
            TranslationTarget.Product => await (from t in context.ProductTranslations.AsNoTracking() join l in context.Languages on t.LanguageId equals l.Id where t.ProductId == id orderby l.Code select new TranslationDto(id, l.Code, t.Name, t.LocalizedDescription, t.RowVersion)).ToListAsync(ct),
            TranslationTarget.Feature => await (from t in context.FeatureTranslations.AsNoTracking() join l in context.Languages on t.LanguageId equals l.Id where t.FeatureId == id orderby l.Code select new TranslationDto(id, l.Code, t.Name, null, t.RowVersion)).ToListAsync(ct),
            TranslationTarget.Specification => await (from t in context.SpecificationTranslations.AsNoTracking() join l in context.Languages on t.LanguageId equals l.Id where t.SpecificationId == id orderby l.Code select new TranslationDto(id, l.Code, t.Name, null, t.RowVersion)).ToListAsync(ct),
            TranslationTarget.ProductSpecification => await (from t in context.ProductSpecificationTranslations.AsNoTracking() join l in context.Languages on t.LanguageId equals l.Id where t.ProductSpecificationId == id orderby l.Code select new TranslationDto(id, l.Code, t.Value, null, t.RowVersion)).ToListAsync(ct),
            _ => throw new ProductOperationException("Invalid translation target.")
        };
    }
    public async Task<long> CreateDefinitionAsync(bool specification, DefinitionRequest request, Guid? userId, CancellationToken ct)
    {
        var fallback = await DefaultAsync(ct);
        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        long id;
        if (specification)
        {
            var entity = new Specification(request.Code, request.Name, request.ValueType, request.Unit);
            entity.SetCreated(userId, DateTime.UtcNow); context.Specifications.Add(entity); await SaveAsync(ct);
            var translation = new SpecificationTranslation(entity.Id, fallback.Id, request.Name); translation.SetCreated(userId, DateTime.UtcNow);
            context.SpecificationTranslations.Add(translation); id = entity.Id;
        }
        else
        {
            var entity = new Feature(request.Code, request.Name); entity.SetCreated(userId, DateTime.UtcNow);
            context.Features.Add(entity); await SaveAsync(ct);
            var translation = new FeatureTranslation(entity.Id, fallback.Id, request.Name); translation.SetCreated(userId, DateTime.UtcNow);
            context.FeatureTranslations.Add(translation); id = entity.Id;
        }
        await SaveAsync(ct); await transaction.CommitAsync(ct); return id;
    }
    public async Task<IReadOnlyCollection<DefinitionDto>> GetDefinitionsAsync(bool specification, DefinitionListRequest request, CancellationToken ct)
    {
        var skip = checked((request.Page - 1) * request.PageSize);
        return specification
            ? await context.Specifications.AsNoTracking().OrderBy(x => x.Code).ThenBy(x => x.Id).Skip(skip).Take(request.PageSize)
                .Select(x => new DefinitionDto(x.Id, x.Code, x.Name, x.ValueType, x.Unit)).ToListAsync(ct)
            : await context.Features.AsNoTracking().OrderBy(x => x.Code).ThenBy(x => x.Id).Skip(skip).Take(request.PageSize)
                .Select(x => new DefinitionDto(x.Id, x.Code, x.Name, null, null)).ToListAsync(ct);
    }
    public async Task<IReadOnlyCollection<SpecificationAssignmentDto>> GetAssignmentsAsync(SpecificationAssignmentsRequest request, CancellationToken ct) =>
        await context.ProductSpecifications.AsNoTracking().Where(x => x.ProductId == request.ProductId)
            .OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id).Skip(checked((request.Page - 1) * request.PageSize)).Take(request.PageSize)
            .Select(x => new SpecificationAssignmentDto(x.Id, x.ProductId, x.SpecificationId, x.TextValue,
                x.NumberValue, x.BooleanValue, x.DateValue, x.CodeValue, x.DisplayOrder, x.RowVersion)).ToListAsync(ct);

    public async Task<long> AssignSpecificationAsync(SpecificationAssignmentRequest request, Guid? userId, CancellationToken ct)
    {
        if (!await context.Products.AnyAsync(x => x.Id == request.ProductId, ct)) throw new ProductNotFoundException(request.ProductId);
        var definition = await context.Specifications.SingleOrDefaultAsync(x => x.Id == request.SpecificationId, ct)
            ?? throw new ProductOperationException("Specification does not exist.");
        var assignment = await context.ProductSpecifications.SingleOrDefaultAsync(x => x.ProductId == request.ProductId && x.SpecificationId == request.SpecificationId, ct);
        if (assignment is not null) Concurrency(assignment, request.RowVersion);
        assignment ??= new ProductSpecification(request.ProductId, request.SpecificationId);
        assignment.SetValue(definition.ValueType, request.TextValue, request.NumberValue, request.BooleanValue, request.DateValue, request.CodeValue, request.DisplayOrder);
        await using var transaction = await context.Database.BeginTransactionAsync(ct);
        if (assignment.Id == 0) { assignment.SetCreated(userId, DateTime.UtcNow); context.ProductSpecifications.Add(assignment); }
        else assignment.SetUpdated(userId, DateTime.UtcNow);
        await SaveAsync(ct);
        if (definition.ValueType == SpecificationValueType.Text)
        {
            var fallback = await DefaultAsync(ct);
            var translation = await context.ProductSpecificationTranslations.SingleOrDefaultAsync(x => x.ProductSpecificationId == assignment.Id && x.LanguageId == fallback.Id, ct);
            if (translation is null) { translation = new ProductSpecificationTranslation(assignment.Id, fallback.Id, assignment.TextValue!); context.ProductSpecificationTranslations.Add(translation); translation.SetCreated(userId, DateTime.UtcNow); }
            else { translation.Update(assignment.TextValue!); translation.SetUpdated(userId, DateTime.UtcNow); }
            await SaveAsync(ct);
        }
        await transaction.CommitAsync(ct); return assignment.Id;
    }
    public async Task<IReadOnlyCollection<MissingTranslationDto>> GetMissingAsync(MissingTranslationsRequest request, CancellationToken ct)
    {
        var code = Language.NormalizeCode(request.LanguageCode);
        var language = await context.Languages.AsNoTracking().SingleOrDefaultAsync(x => x.Code == code, ct)
            ?? throw new ProductOperationException("Language does not exist.");
        var skip = checked((request.Page - 1) * request.PageSize);
        // Order and page entity rows before constructing the response record so SQL Server can translate the query.
        return request.Target switch
        {
            TranslationTarget.Product => await context.Products.AsNoTracking()
                .Where(x => !context.ProductTranslations.Any(t => t.ProductId == x.Id && t.LanguageId == language.Id))
                .OrderBy(x => x.Id).Skip(skip).Take(request.PageSize)
                .Select(x => new MissingTranslationDto(x.Id, x.Name)).ToListAsync(ct),
            TranslationTarget.Feature => await context.Features.AsNoTracking()
                .Where(x => !context.FeatureTranslations.Any(t => t.FeatureId == x.Id && t.LanguageId == language.Id))
                .OrderBy(x => x.Id).Skip(skip).Take(request.PageSize)
                .Select(x => new MissingTranslationDto(x.Id, x.Name)).ToListAsync(ct),
            TranslationTarget.Specification => await context.Specifications.AsNoTracking()
                .Where(x => !context.SpecificationTranslations.Any(t => t.SpecificationId == x.Id && t.LanguageId == language.Id))
                .OrderBy(x => x.Id).Skip(skip).Take(request.PageSize)
                .Select(x => new MissingTranslationDto(x.Id, x.Name)).ToListAsync(ct),
            TranslationTarget.ProductSpecification => await context.ProductSpecifications.AsNoTracking()
                .Where(x => x.TextValue != null && !context.ProductSpecificationTranslations.Any(t => t.ProductSpecificationId == x.Id && t.LanguageId == language.Id))
                .OrderBy(x => x.Id).Skip(skip).Take(request.PageSize)
                .Select(x => new MissingTranslationDto(x.Id, x.TextValue!)).ToListAsync(ct),
            _ => throw new ProductOperationException("Invalid translation target.")
        };
    }
}
