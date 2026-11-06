using Dapper;
using MAHAK.Integration.Api.Contracts.Invoices;
using MAHAK.Integration.Api.Data.Queries;
using MAHAK.Integration.Api.Security;

namespace MAHAK.Integration.Api.Data.Repositories;

public interface IInvoiceRepository
{
    Task<IReadOnlyList<CurrentInvoiceLine>> GetCurrentNonReturnedAsync(
        BrandContext brand,
        int take,
        CancellationToken cancellationToken);
}

public sealed class InvoiceRepository(ISqlConnectionFactory connections) : IInvoiceRepository
{
    public const int ProductCodeThreshold = 10_000;

    public async Task<IReadOnlyList<CurrentInvoiceLine>> GetCurrentNonReturnedAsync(
        BrandContext brand,
        int take,
        CancellationToken cancellationToken)
    {
        await using var connection = connections.Create(brand);
        var command = new CommandDefinition(
            InvoiceSql.CurrentNonReturnedProjection,
            new { Take = take, ProductCodeThreshold },
            commandTimeout: brand.CommandTimeoutSeconds,
            cancellationToken: cancellationToken);
        return (await connection.QueryAsync<CurrentInvoiceLine>(command)).AsList();
    }
}
