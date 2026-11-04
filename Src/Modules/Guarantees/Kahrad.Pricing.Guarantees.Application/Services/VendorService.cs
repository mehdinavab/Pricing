using AutoMapper;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Guarantees.Application.Abstractions.Persistence;
using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;
using Kahrad.Pricing.Guarantees.Application.Exceptions;
using Kahrad.Pricing.Guarantees.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace Kahrad.Pricing.Guarantees.Application.Services;

public sealed class VendorService(
    IVendorRepository vendorRepository,
    IMapper mapper,
    ISieveProcessor sieveProcessor) : IVendorService
{
    public async Task<VendorDto?> GetByIdAsync(
        long vendorId,
        CancellationToken cancellationToken = default)
    {
        var vendor = await vendorRepository.GetByIdAsync(
            vendorId,
            includeGuarantees: false,
            cancellationToken);

        return vendor is null ? null : mapper.Map<VendorDto>(vendor);
    }

    public async Task<VendorDetailDto?> GetDetailByIdAsync(
        long vendorId,
        CancellationToken cancellationToken = default)
    {
        var vendor = await vendorRepository.GetByIdAsync(
            vendorId,
            includeGuarantees: true,
            cancellationToken);

        return vendor is null ? null : mapper.Map<VendorDetailDto>(vendor);
    }

    public async Task<VendorDto?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default)
    {
        var vendor = await vendorRepository.GetByCodeAsync(
            code,
            includeGuarantees: false,
            cancellationToken);

        return vendor is null ? null : mapper.Map<VendorDto>(vendor);
    }

    public async Task<IReadOnlyCollection<VendorDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        var vendors = await vendorRepository.GetAllAsync(
            includeGuarantees: false,
            cancellationToken);

        return mapper.Map<IReadOnlyCollection<VendorDto>>(vendors);
    }

    public async Task<BaseResponsePaginated<VendorDto>> GetPagedAsync(
        VendorQueryDto filter,
        CancellationToken cancellationToken = default)
    {
        EnsureStableSort(filter);

        var query = vendorRepository.GetQueryable().AsNoTracking();

        var totalCount = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: false,
                applyPagination: false)
            .CountAsync(cancellationToken);

        var vendors = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: true,
                applyPagination: true)
            .ToListAsync(cancellationToken);

        var data = mapper.Map<List<VendorDto>>(vendors);

        return BaseResponsePaginated<VendorDto>.Success(
            data,
            filter.Page ?? 1,
            filter.PageSize ?? 20,
            totalCount);
    }

    public async Task<long> CreateAsync(
        CreateVendorRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        if (await vendorRepository.ExistsByCodeAsync(request.Code, null, cancellationToken))
        {
            throw new GuaranteeOperationException(
                $"A vendor with code '{request.Code}' already exists.");
        }

        var utcNow = DateTime.UtcNow;
        var vendor = new Vendor(
            request.Name,
            request.Code,
            request.ContactPerson,
            request.PhoneNumber,
            request.Email,
            request.Address,
            request.Website,
            request.Description,
            request.IsActive);

        vendor.SetCreated(currentUserId, utcNow);

        await vendorRepository.AddAsync(vendor, cancellationToken);
        return vendor.Id;
    }

    public async Task UpdateAsync(
        long vendorId,
        UpdateVendorRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var vendor = await vendorRepository.GetByIdAsync(
            vendorId,
            includeGuarantees: false,
            cancellationToken);

        if (vendor is null)
            throw new VendorNotFoundException(vendorId);

        if (await vendorRepository.ExistsByCodeAsync(request.Code, vendorId, cancellationToken))
        {
            throw new GuaranteeOperationException(
                $"Another vendor with code '{request.Code}' already exists.");
        }

        vendor.Update(
            request.Name,
            request.Code,
            request.ContactPerson,
            request.PhoneNumber,
            request.Email,
            request.Address,
            request.Website,
            request.Description,
            DateTime.UtcNow,
            currentUserId);

        await vendorRepository.UpdateAsync(vendor, cancellationToken);
    }

    public async Task DeleteAsync(
        long vendorId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var vendor = await vendorRepository.GetByIdAsync(
            vendorId,
            includeGuarantees: false,
            cancellationToken);

        if (vendor is null)
            throw new VendorNotFoundException(vendorId);

        vendor.SetDeleted(currentUserId, DateTime.UtcNow);
        await vendorRepository.DeleteAsync(vendor, cancellationToken);
    }

    public async Task ChangeStatusAsync(
        long vendorId,
        bool isActive,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var vendor = await vendorRepository.GetByIdAsync(
            vendorId,
            includeGuarantees: false,
            cancellationToken);

        if (vendor is null)
            throw new VendorNotFoundException(vendorId);

        var utcNow = DateTime.UtcNow;
        if (isActive)
            vendor.Activate(utcNow, currentUserId);
        else
            vendor.Deactivate(utcNow, currentUserId);

        await vendorRepository.UpdateAsync(vendor, cancellationToken);
    }

    private static void EnsureStableSort(VendorQueryDto filter)
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
