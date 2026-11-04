using Kahrad.Pricing.BuildingBlocks.Application.DTOs.Responses;
using Kahrad.Pricing.Identity.Application.Abstractions.Persistence;
using Kahrad.Pricing.Identity.Application.DTOs.Auditing;
using Kahrad.Pricing.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Sieve.Services;

namespace Kahrad.Pricing.Identity.Infrastructure.Persistence.Repositories;

public sealed class AuditLogRepository(
    IdentityDbContext context,
    ISieveProcessor sieveProcessor) : IAuditLogRepository
{
    public async Task AddAsync(
        AuditLog auditLog,
        CancellationToken cancellationToken = default)
    {
        await context.AuditLogs.AddAsync(auditLog, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AuditLog?> GetByIdAsync(
        long id,
        CancellationToken cancellationToken = default)
    {
        return await context.AuditLogs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<BaseResponsePaginated<AuditLog>> GetPagedAsync(
        AuditLogQueryDto filter,
        CancellationToken cancellationToken = default)
    {
        EnsureStableSort(filter);
        var query = context.AuditLogs.AsNoTracking();

        // Apply filtering and sorting before counting.
        var filteredQuery = sieveProcessor.Apply(
            filter,
            query,
            applyFiltering: true,
            applySorting: true,
            applyPagination: false);

        var totalCount = await filteredQuery.CountAsync(cancellationToken);

        // Apply pagination.
        var pagedQuery = sieveProcessor.Apply(
            filter,
            query,
            applyFiltering: true,
            applySorting: true,
            applyPagination: true);

        var data = await pagedQuery.ToListAsync(cancellationToken);

        return BaseResponsePaginated<AuditLog>.Success(
            data,
            filter.Page ?? 1,
            filter.PageSize ?? 20,
            totalCount);
    }

    private static void EnsureStableSort(AuditLogQueryDto filter)
    {
        var sortFields = filter.Sorts?
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(x => x.TrimStart('+', '-'));

        if (sortFields?.Contains("Id", StringComparer.OrdinalIgnoreCase) == true)
            return;

        filter.Sorts = string.IsNullOrWhiteSpace(filter.Sorts)
            ? "Id"
            : $"{filter.Sorts},Id";
    }
}