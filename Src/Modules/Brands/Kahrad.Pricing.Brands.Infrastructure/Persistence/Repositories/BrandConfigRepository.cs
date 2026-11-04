using Kahrad.Pricing.Brands.Application.Abstractions.Persistence;
using Kahrad.Pricing.Brands.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Brands.Infrastructure.Persistence.Repositories;

public sealed class BrandConfigRepository(BrandsDbContext context) : IBrandConfigRepository
{
    public async Task<IReadOnlyCollection<BrandConfig>> GetByBrandIdAsync(
        long brandId,
        CancellationToken cancellationToken = default)
    {
        return await context.BrandConfigs
            .AsNoTracking()
            .Where(x => x.BrandId == brandId)
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task<BrandConfig?> GetByKeyAsync(
        long brandId,
        string key,
        CancellationToken cancellationToken = default)
    {
        var normalizedKey = key.Trim();
        return await context.BrandConfigs
            .FirstOrDefaultAsync(
                x => x.BrandId == brandId && x.Key == normalizedKey,
                cancellationToken);
    }

    public async Task SetConfigsAsync(
        long brandId,
        IEnumerable<BrandConfig> configs,
        CancellationToken cancellationToken = default)
    {
        var existingConfigs = await context.BrandConfigs
            .Where(x => x.BrandId == brandId)
            .ToListAsync(cancellationToken);

        var existingMap = existingConfigs.ToDictionary(x => x.Key, StringComparer.OrdinalIgnoreCase);

        foreach (var config in configs)
        {
            if (existingMap.TryGetValue(config.Key, out var existing))
            {
                existing.Update(
                    config.DisplayName,
                    config.Value,
                    config.DataType,
                    config.Category,
                    config.Description,
                    config.IsActive,
                    DateTime.UtcNow);
            }
            else
            {
                await context.BrandConfigs.AddAsync(config, cancellationToken);
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(
        long brandId,
        string key,
        CancellationToken cancellationToken = default)
    {
        var normalizedKey = key.Trim();
        var config = await context.BrandConfigs
            .FirstOrDefaultAsync(
                x => x.BrandId == brandId && x.Key == normalizedKey,
                cancellationToken);

        if (config is not null)
        {
            context.BrandConfigs.Remove(config);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
