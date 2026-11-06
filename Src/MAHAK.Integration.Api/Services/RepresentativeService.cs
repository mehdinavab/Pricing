using System.Diagnostics;
using MAHAK.Integration.Api.Contracts.Common;
using MAHAK.Integration.Api.Contracts.Representatives;
using MAHAK.Integration.Api.Data.Repositories;
using MAHAK.Integration.Api.Security;

namespace MAHAK.Integration.Api.Services;

public sealed class RepresentativeService(
    IRepresentativeRepository repository,
    BoundedSyncPolicy syncPolicy,
    ILogger<RepresentativeService> logger)
{
    public async Task<SyncResponse<AccountingRepresentative>> SyncAsync(
        BrandContext brand,
        SyncRequest request,
        CancellationToken cancellationToken)
    {
        var pageSize = syncPolicy.Validate(request);
        var timer = Stopwatch.StartNew();
        var rows = await repository.GetAsync(brand, pageSize + 1, cancellationToken);
        logger.LogInformation(
            "MAHAK representative synchronization completed for brand {BrandCode} with {RowCount} rows in {ElapsedMilliseconds} ms",
            brand.BrandCode,
            Math.Min(rows.Count, pageSize),
            timer.ElapsedMilliseconds);
        return BoundedSyncPolicy.Complete(rows, pageSize);
    }
}
