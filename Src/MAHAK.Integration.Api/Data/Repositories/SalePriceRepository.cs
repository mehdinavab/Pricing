using System.Data;
using Dapper;
using MAHAK.Integration.Api.Contracts.Prices;
using MAHAK.Integration.Api.Data.Queries;
using MAHAK.Integration.Api.Security;

namespace MAHAK.Integration.Api.Data.Repositories;

public enum SalePriceUpdateResult
{
    NotFound,
    Updated,
    MultipleMatches
}

public interface ISalePriceRepository
{
    Task<SalePriceUpdateResult> UpdateAsync(
        BrandContext brand,
        SetSalePriceRequest request,
        CancellationToken cancellationToken);
}

public sealed class SalePriceRepository(ISqlConnectionFactory connections)
    : ISalePriceRepository
{
    public async Task<SalePriceUpdateResult> UpdateAsync(
        BrandContext brand,
        SetSalePriceRequest request,
        CancellationToken cancellationToken)
    {
        await using var connection = connections.Create(brand);
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(
            IsolationLevel.ReadCommitted,
            cancellationToken);
        var command = new CommandDefinition(
            SalePriceSql.Update,
            new
            {
                request.GoodCode,
                request.ConsumerPrice,
                request.ConsumerPriceDollar
            },
            transaction,
            brand.CommandTimeoutSeconds,
            cancellationToken: cancellationToken);
        var affectedRows = await connection.ExecuteAsync(command);
        if (affectedRows > 1)
        {
            await transaction.RollbackAsync(cancellationToken);
            return SalePriceUpdateResult.MultipleMatches;
        }

        await transaction.CommitAsync(cancellationToken);
        return affectedRows == 0
            ? SalePriceUpdateResult.NotFound
            : SalePriceUpdateResult.Updated;
    }
}
