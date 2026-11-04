using AutoMapper;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Guarantees.Application.Abstractions.Persistence;
using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;
using Kahrad.Pricing.Guarantees.Application.Exceptions;
using Kahrad.Pricing.Guarantees.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace Kahrad.Pricing.Guarantees.Application.Services;

public sealed class GuaranteeService(
    IGuaranteeRepository guaranteeRepository,
    IVendorRepository vendorRepository,
    IMapper mapper,
    ISieveProcessor sieveProcessor) : IGuaranteeService
{
    public async Task<GuaranteeDto?> GetByIdAsync(
        long guaranteeId,
        CancellationToken cancellationToken = default)
    {
        var guarantee = await guaranteeRepository.GetByIdAsync(
            guaranteeId,
            includeVendor: true,
            includeCertificates: false,
            cancellationToken);

        return guarantee is null ? null : mapper.Map<GuaranteeDto>(guarantee);
    }

    public async Task<GuaranteeDetailDto?> GetDetailByIdAsync(
        long guaranteeId,
        CancellationToken cancellationToken = default)
    {
        var guarantee = await guaranteeRepository.GetByIdAsync(
            guaranteeId,
            includeVendor: true,
            includeCertificates: true,
            cancellationToken);

        return guarantee is null ? null : mapper.Map<GuaranteeDetailDto>(guarantee);
    }

    public async Task<GuaranteeDto?> GetByCodeAsync(
        long vendorId,
        string code,
        CancellationToken cancellationToken = default)
    {
        var guarantee = await guaranteeRepository.GetByCodeAsync(
            vendorId,
            code,
            includeVendor: true,
            cancellationToken);

        return guarantee is null ? null : mapper.Map<GuaranteeDto>(guarantee);
    }

    public async Task<IReadOnlyCollection<GuaranteeDto>> GetByVendorIdAsync(
        long vendorId,
        CancellationToken cancellationToken = default)
    {
        if (!await vendorRepository.ExistsAsync(vendorId, cancellationToken))
            throw new VendorNotFoundException(vendorId);

        var guarantees = await guaranteeRepository.GetByVendorIdAsync(vendorId, cancellationToken);
        return mapper.Map<IReadOnlyCollection<GuaranteeDto>>(guarantees);
    }

    public async Task<BaseResponsePaginated<GuaranteeDto>> GetPagedAsync(
        GuaranteeQueryDto filter,
        CancellationToken cancellationToken = default)
    {
        EnsureStableSort(filter);

        var query = guaranteeRepository.GetQueryable().AsNoTracking();

        if (filter.VendorId.HasValue)
        {
            query = query.Where(x => x.VendorId == filter.VendorId.Value);
        }

        var totalCount = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: false,
                applyPagination: false)
            .CountAsync(cancellationToken);

        var guarantees = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: true,
                applyPagination: true)
            .Include(x => x.Vendor)
            .ToListAsync(cancellationToken);

        var data = mapper.Map<List<GuaranteeDto>>(guarantees);

        return BaseResponsePaginated<GuaranteeDto>.Success(
            data,
            filter.Page ?? 1,
            filter.PageSize ?? 20,
            totalCount);
    }

    public async Task<long> CreateAsync(
        CreateGuaranteeRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        if (!await vendorRepository.ExistsAsync(request.VendorId, cancellationToken))
            throw new VendorNotFoundException(request.VendorId);

        if (await guaranteeRepository.ExistsByCodeAsync(request.VendorId, request.Code, null, cancellationToken))
        {
            throw new GuaranteeOperationException(
                $"A guarantee with code '{request.Code}' already exists for Vendor '{request.VendorId}'.");
        }

        var utcNow = DateTime.UtcNow;
        var guarantee = new Guarantee(
            request.VendorId,
            request.Name,
            request.Code,
            request.DurationMonths,
            request.Description,
            request.DurationDays,
            request.TermsAndConditions,
            request.IsActive);

        guarantee.SetCreated(currentUserId, utcNow);

        await guaranteeRepository.AddAsync(guarantee, cancellationToken);
        return guarantee.Id;
    }

    public async Task UpdateAsync(
        long guaranteeId,
        UpdateGuaranteeRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var guarantee = await guaranteeRepository.GetByIdAsync(
            guaranteeId,
            includeVendor: false,
            includeCertificates: false,
            cancellationToken);

        if (guarantee is null)
            throw new GuaranteeNotFoundException(guaranteeId);

        if (await guaranteeRepository.ExistsByCodeAsync(guarantee.VendorId, request.Code, guaranteeId, cancellationToken))
        {
            throw new GuaranteeOperationException(
                $"Another guarantee with code '{request.Code}' already exists for Vendor '{guarantee.VendorId}'.");
        }

        guarantee.Update(
            request.Name,
            request.Code,
            request.DurationMonths,
            request.Description,
            request.DurationDays,
            request.TermsAndConditions,
            DateTime.UtcNow,
            currentUserId);

        await guaranteeRepository.UpdateAsync(guarantee, cancellationToken);
    }

    public async Task DeleteAsync(
        long guaranteeId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var guarantee = await guaranteeRepository.GetByIdAsync(
            guaranteeId,
            includeVendor: false,
            includeCertificates: false,
            cancellationToken);

        if (guarantee is null)
            throw new GuaranteeNotFoundException(guaranteeId);

        guarantee.SetDeleted(currentUserId, DateTime.UtcNow);
        await guaranteeRepository.DeleteAsync(guarantee, cancellationToken);
    }

    public async Task ChangeStatusAsync(
        long guaranteeId,
        bool isActive,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var guarantee = await guaranteeRepository.GetByIdAsync(
            guaranteeId,
            includeVendor: false,
            includeCertificates: false,
            cancellationToken);

        if (guarantee is null)
            throw new GuaranteeNotFoundException(guaranteeId);

        var utcNow = DateTime.UtcNow;
        if (isActive)
            guarantee.Activate(utcNow, currentUserId);
        else
            guarantee.Deactivate(utcNow, currentUserId);

        await guaranteeRepository.UpdateAsync(guarantee, cancellationToken);
    }

    private static void EnsureStableSort(GuaranteeQueryDto filter)
    {
        var sortFields = filter.Sorts?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.TrimStart('+', '-'));

        if (sortFields?.Contains("Id", StringComparer.OrdinalIgnoreCase) == true)
            return;

        filter.Sorts = string.IsNullOrWhiteSpace(filter.Sorts)
            ? "Name,Id"
            : $"{filter.Sorts},Id";
    }
}
