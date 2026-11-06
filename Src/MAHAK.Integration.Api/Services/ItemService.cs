using System.Diagnostics;
using MAHAK.Integration.Api.Contracts.Common;
using MAHAK.Integration.Api.Contracts.Items;
using MAHAK.Integration.Api.Data.Repositories;
using MAHAK.Integration.Api.Errors;
using MAHAK.Integration.Api.Security;

namespace MAHAK.Integration.Api.Services;

public sealed class ItemService(
    IItemRepository repository,
    BoundedSyncPolicy syncPolicy,
    ILogger<ItemService> logger)
{
    public async Task<SyncResponse<AccountingItem>> SyncAsync(
        BrandContext brand,
        ItemSyncRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Scope is null)
            throw new RequestValidationException("Scope is required.");
        if (request.Scope == ItemSyncScope.AllItems)
        {
            throw new SchemaConfirmationRequiredException(
                "AllItems requires confirmed GoodInf projection, price columns, filters, and a stable ordering key.");
        }
        if (request.Scope != ItemSyncScope.ReturnedItemsOnly)
            throw new RequestValidationException("The requested item synchronization scope is invalid.");

        var pageSize = syncPolicy.Validate(request);
        var timer = Stopwatch.StartNew();
        var rows = await repository.GetReturnedItemsAsync(
            brand,
            pageSize + 1,
            cancellationToken);
        logger.LogInformation(
            "MAHAK returned-item synchronization completed for brand {BrandCode} with {RowCount} rows in {ElapsedMilliseconds} ms",
            brand.BrandCode,
            Math.Min(rows.Count, pageSize),
            timer.ElapsedMilliseconds);
        return BoundedSyncPolicy.Complete(rows, pageSize);
    }
}
