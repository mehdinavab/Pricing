using Dapper;
using MAHAK.Integration.Api.Contracts.Items;
using MAHAK.Integration.Api.Data.Queries;
using MAHAK.Integration.Api.Security;

namespace MAHAK.Integration.Api.Data.Repositories;

public interface IItemRepository
{
    Task<IReadOnlyList<AccountingItem>> GetReturnedItemsAsync(
        BrandContext brand,
        int take,
        CancellationToken cancellationToken);
}

public sealed class ItemRepository(ISqlConnectionFactory connections) : IItemRepository
{
    public async Task<IReadOnlyList<AccountingItem>> GetReturnedItemsAsync(
        BrandContext brand,
        int take,
        CancellationToken cancellationToken)
    {
        await using var connection = connections.Create(brand);
        var command = new CommandDefinition(
            ItemSql.ReturnedItemsOnly,
            new { Take = take },
            commandTimeout: brand.CommandTimeoutSeconds,
            cancellationToken: cancellationToken);
        return (await connection.QueryAsync<AccountingItem>(command)).AsList();
    }
}
