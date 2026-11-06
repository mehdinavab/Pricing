using System.Diagnostics;
using MAHAK.Integration.Api.Contracts.Common;
using MAHAK.Integration.Api.Contracts.Invoices;
using MAHAK.Integration.Api.Data.Repositories;
using MAHAK.Integration.Api.Security;

namespace MAHAK.Integration.Api.Services;

public sealed class InvoiceService(
    IInvoiceRepository repository,
    BoundedSyncPolicy syncPolicy,
    ILogger<InvoiceService> logger)
{
    public async Task<SyncResponse<CurrentInvoiceLine>> SyncAsync(
        BrandContext brand,
        SyncRequest request,
        CancellationToken cancellationToken)
    {
        var pageSize = syncPolicy.Validate(request);
        var timer = Stopwatch.StartNew();
        var rows = await repository.GetCurrentNonReturnedAsync(
            brand,
            pageSize + 1,
            cancellationToken);
        logger.LogInformation(
            "MAHAK invoice synchronization completed for brand {BrandCode} with {RowCount} rows in {ElapsedMilliseconds} ms",
            brand.BrandCode,
            Math.Min(rows.Count, pageSize),
            timer.ElapsedMilliseconds);
        return BoundedSyncPolicy.Complete(rows, pageSize);
    }
}
