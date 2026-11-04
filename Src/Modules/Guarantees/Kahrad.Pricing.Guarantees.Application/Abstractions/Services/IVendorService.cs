using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Guarantees.Application.DTOs.Vendors;

namespace Kahrad.Pricing.Guarantees.Application.Abstractions.Services;

public interface IVendorService
{
    Task<VendorDto?> GetByIdAsync(
        long vendorId,
        CancellationToken cancellationToken = default);

    Task<VendorDetailDto?> GetDetailByIdAsync(
        long vendorId,
        CancellationToken cancellationToken = default);

    Task<VendorDto?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<VendorDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<BaseResponsePaginated<VendorDto>> GetPagedAsync(
        VendorQueryDto filter,
        CancellationToken cancellationToken = default);

    Task<long> CreateAsync(
        CreateVendorRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        long vendorId,
        UpdateVendorRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long vendorId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        long vendorId,
        bool isActive,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);
}
