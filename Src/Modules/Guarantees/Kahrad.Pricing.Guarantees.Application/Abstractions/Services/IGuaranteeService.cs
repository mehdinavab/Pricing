using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Guarantees.Application.DTOs.Guarantees;

namespace Kahrad.Pricing.Guarantees.Application.Abstractions.Services;

public interface IGuaranteeService
{
    Task<GuaranteeDto?> GetByIdAsync(
        long guaranteeId,
        CancellationToken cancellationToken = default);

    Task<GuaranteeDetailDto?> GetDetailByIdAsync(
        long guaranteeId,
        CancellationToken cancellationToken = default);

    Task<GuaranteeDto?> GetByCodeAsync(
        long vendorId,
        string code,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GuaranteeDto>> GetByVendorIdAsync(
        long vendorId,
        CancellationToken cancellationToken = default);

    Task<BaseResponsePaginated<GuaranteeDto>> GetPagedAsync(
        GuaranteeQueryDto filter,
        CancellationToken cancellationToken = default);

    Task<long> CreateAsync(
        CreateGuaranteeRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task UpdateAsync(
        long guaranteeId,
        UpdateGuaranteeRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        long guaranteeId,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task ChangeStatusAsync(
        long guaranteeId,
        bool isActive,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);
}
