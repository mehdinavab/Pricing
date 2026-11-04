using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;

namespace Kahrad.Pricing.Guarantees.Application.Abstractions.Services;

public interface IGuaranteeCertificateService
{
    Task<GuaranteeCertificateDto?> GetByIdAsync(
        long certificateId,
        CancellationToken cancellationToken = default);

    Task<GuaranteeCertificateDto?> GetByCertificateNumberAsync(
        string certificateNumber,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyCollection<GuaranteeCertificateDto>> GetByProductIdAsync(
        long productId,
        CancellationToken cancellationToken = default);

    Task<BaseResponsePaginated<GuaranteeCertificateDto>> GetPagedAsync(
        CertificateQueryDto filter,
        CancellationToken cancellationToken = default);

    Task<GuaranteeCertificateDto> IssueCertificateAsync(
        IssueCertificateRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task RevokeCertificateAsync(
        long certificateId,
        RevokeCertificateRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);

    Task ExtendCertificateAsync(
        long certificateId,
        ExtendCertificateRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default);
}
