using AutoMapper;
using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Guarantees.Application.Abstractions.Persistence;
using Kahrad.Pricing.Guarantees.Application.Abstractions.Services;
using Kahrad.Pricing.Guarantees.Application.DTOs.Certificates;
using Kahrad.Pricing.Guarantees.Application.Exceptions;
using Kahrad.Pricing.Guarantees.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace Kahrad.Pricing.Guarantees.Application.Services;

public sealed class GuaranteeCertificateService(
    IGuaranteeCertificateRepository certificateRepository,
    IGuaranteeRepository guaranteeRepository,
    IMapper mapper,
    ISieveProcessor sieveProcessor) : IGuaranteeCertificateService
{
    public async Task<GuaranteeCertificateDto?> GetByIdAsync(
        long certificateId,
        CancellationToken cancellationToken = default)
    {
        var certificate = await certificateRepository.GetByIdAsync(
            certificateId,
            includeGuarantee: true,
            cancellationToken);

        return certificate is null ? null : mapper.Map<GuaranteeCertificateDto>(certificate);
    }

    public async Task<GuaranteeCertificateDto?> GetByCertificateNumberAsync(
        string certificateNumber,
        CancellationToken cancellationToken = default)
    {
        var certificate = await certificateRepository.GetByCertificateNumberAsync(
            certificateNumber,
            includeGuarantee: true,
            cancellationToken);

        return certificate is null ? null : mapper.Map<GuaranteeCertificateDto>(certificate);
    }

    public async Task<IReadOnlyCollection<GuaranteeCertificateDto>> GetByProductIdAsync(
        long productId,
        CancellationToken cancellationToken = default)
    {
        var certificates = await certificateRepository.GetByProductIdAsync(
            productId,
            cancellationToken);

        return mapper.Map<IReadOnlyCollection<GuaranteeCertificateDto>>(certificates);
    }

    public async Task<BaseResponsePaginated<GuaranteeCertificateDto>> GetPagedAsync(
        CertificateQueryDto filter,
        CancellationToken cancellationToken = default)
    {
        EnsureStableSort(filter);

        var query = certificateRepository.GetQueryable().AsNoTracking();

        if (filter.ProductId.HasValue)
            query = query.Where(x => x.ProductId == filter.ProductId.Value);

        if (filter.GuaranteeId.HasValue)
            query = query.Where(x => x.GuaranteeId == filter.GuaranteeId.Value);

        if (filter.StoreId.HasValue)
            query = query.Where(x => x.StoreId == filter.StoreId.Value);

        if (filter.Status.HasValue)
            query = query.Where(x => x.Status == filter.Status.Value);

        var totalCount = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: false,
                applyPagination: false)
            .CountAsync(cancellationToken);

        var certificates = await sieveProcessor
            .Apply(
                filter,
                query,
                applyFiltering: true,
                applySorting: true,
                applyPagination: true)
            .Include(x => x.Guarantee)
                .ThenInclude(g => g.Vendor)
            .ToListAsync(cancellationToken);

        var data = mapper.Map<List<GuaranteeCertificateDto>>(certificates);

        return BaseResponsePaginated<GuaranteeCertificateDto>.Success(
            data,
            filter.Page ?? 1,
            filter.PageSize ?? 20,
            totalCount);
    }

    public async Task<GuaranteeCertificateDto> IssueCertificateAsync(
        IssueCertificateRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var guarantee = await guaranteeRepository.GetByIdAsync(
            request.GuaranteeId,
            includeVendor: true,
            includeCertificates: false,
            cancellationToken);

        if (guarantee is null)
            throw new GuaranteeNotFoundException(request.GuaranteeId);

        if (!guarantee.IsActive)
            throw new GuaranteeOperationException("Cannot issue a certificate for an inactive guarantee plan.");

        var certNumber = string.IsNullOrWhiteSpace(request.CertificateNumber)
            ? GenerateCertificateNumber(request.ProductId)
            : request.CertificateNumber.Trim().ToUpperInvariant();

        if (await certificateRepository.ExistsByCertificateNumberAsync(certNumber, null, cancellationToken))
        {
            throw new GuaranteeOperationException(
                $"A certificate with number '{certNumber}' already exists.");
        }

        var expiryDate = guarantee.CalculateExpiryDate(request.PurchaseDate);
        var utcNow = DateTime.UtcNow;

        var certificate = new GuaranteeCertificate(
            request.GuaranteeId,
            certNumber,
            request.ProductId,
            request.PurchaseDate,
            expiryDate,
            startDate: request.PurchaseDate,
            productSerialNumber: request.ProductSerialNumber,
            storeId: request.StoreId,
            customerName: request.CustomerName,
            customerPhoneNumber: request.CustomerPhoneNumber,
            customerNationalCode: request.CustomerNationalCode,
            description: request.Description);

        certificate.SetCreated(currentUserId, utcNow);

        await certificateRepository.AddAsync(certificate, cancellationToken);

        // Fetch back with navigation properties populated
        var result = await certificateRepository.GetByIdAsync(
            certificate.Id,
            includeGuarantee: true,
            cancellationToken);

        return mapper.Map<GuaranteeCertificateDto>(result ?? certificate);
    }

    public async Task RevokeCertificateAsync(
        long certificateId,
        RevokeCertificateRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var certificate = await certificateRepository.GetByIdAsync(
            certificateId,
            includeGuarantee: false,
            cancellationToken);

        if (certificate is null)
            throw new CertificateNotFoundException(certificateId);

        certificate.Revoke(request.Reason, DateTime.UtcNow, currentUserId);
        await certificateRepository.UpdateAsync(certificate, cancellationToken);
    }

    public async Task ExtendCertificateAsync(
        long certificateId,
        ExtendCertificateRequest request,
        Guid? currentUserId = null,
        CancellationToken cancellationToken = default)
    {
        var certificate = await certificateRepository.GetByIdAsync(
            certificateId,
            includeGuarantee: false,
            cancellationToken);

        if (certificate is null)
            throw new CertificateNotFoundException(certificateId);

        certificate.Extend(request.AdditionalMonths, DateTime.UtcNow, currentUserId);
        await certificateRepository.UpdateAsync(certificate, cancellationToken);
    }

    private static string GenerateCertificateNumber(long productId)
    {
        return $"GC-{productId}-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
    }

    private static void EnsureStableSort(CertificateQueryDto filter)
    {
        var sortFields = filter.Sorts?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.TrimStart('+', '-'));

        if (sortFields?.Contains("Id", StringComparer.OrdinalIgnoreCase) == true)
            return;

        filter.Sorts = string.IsNullOrWhiteSpace(filter.Sorts)
            ? "PurchaseDate,Id"
            : $"{filter.Sorts},Id";
    }
}
