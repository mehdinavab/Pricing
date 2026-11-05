using Kahrad.Pricing.Products.Application.Abstractions.Persistence;
using Kahrad.Pricing.Products.Application.Exceptions;
using Kahrad.Pricing.Products.Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.Products.Infrastructure.Persistence.Repositories;

public sealed class ItemCatalogStore(ProductsDbContext db) : IItemCatalogStore
{
    public async Task<long> CreateItemAsync(string code, string name, string unitCode, CancellationToken ct)
    {
        var item = new Item(code, name, unitCode);
        db.Items.Add(item);
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException e) when (e.InnerException is SqlException { Number: 2601 or 2627 })
        { throw new ProductOperationException("Local item code already exists."); }
        return item.Id;
    }

    public async Task<IReadOnlyList<LocalItemView>> SearchItemsAsync(string? search, int page, int pageSize, CancellationToken ct)
    {
        var query = db.Items.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Code.Contains(search) || x.Name.Contains(search));
        return await query.OrderBy(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new LocalItemView(x.Id, x.Code, x.Name, x.UnitCode)).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ProductItemView>> GetCompositionAsync(long productId, CancellationToken ct) =>
        await (from line in db.ProductItems.AsNoTracking()
               join item in db.Items.AsNoTracking() on line.ItemId equals item.Id
               where line.ProductId == productId
               orderby line.ItemId
               select new ProductItemView(line.ItemId, item.Code, item.UnitCode, line.QuantityUsed)).ToListAsync(ct);

    public async Task ReplaceCompositionAsync(long productId, IReadOnlyList<ProductItemInput> items, CancellationToken ct)
    {
        if (!await db.Products.AnyAsync(x => x.Id == productId, ct)) throw new ProductNotFoundException(productId);
        var ids = items.Select(x => x.ItemId).ToArray();
        if (await db.Items.CountAsync(x => ids.Contains(x.Id), ct) != ids.Length)
            throw new ProductOperationException("Product composition contains an unknown local item.");
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var existing = await db.ProductItems.Where(x => x.ProductId == productId).ToListAsync(ct);
        db.ProductItems.RemoveRange(existing);
        await db.SaveChangesAsync(ct);
        db.ProductItems.AddRange(items.Select(x => new ProductItem(productId, x.ItemId, x.QuantityUsed)));
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
    }
}
