using Dapper;
using MAHAK.Integration.Api.Contracts.Representatives;
using MAHAK.Integration.Api.Data.Queries;
using MAHAK.Integration.Api.Security;

namespace MAHAK.Integration.Api.Data.Repositories;

public interface IRepresentativeRepository
{
    Task<IReadOnlyList<AccountingRepresentative>> GetAsync(
        BrandContext brand,
        int take,
        CancellationToken cancellationToken);
}

public sealed class RepresentativeRepository(ISqlConnectionFactory connections)
    : IRepresentativeRepository
{
    public async Task<IReadOnlyList<AccountingRepresentative>> GetAsync(
        BrandContext brand,
        int take,
        CancellationToken cancellationToken)
    {
        await using var connection = connections.Create(brand);
        var command = new CommandDefinition(
            RepresentativeSql.BoundedProjection,
            new { Take = take },
            commandTimeout: brand.CommandTimeoutSeconds,
            cancellationToken: cancellationToken);
        return (await connection.QueryAsync<AccountingRepresentative>(command)).AsList();
    }
}
