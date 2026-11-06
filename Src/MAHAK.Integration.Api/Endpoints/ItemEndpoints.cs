using MAHAK.Integration.Api.Contracts.Items;
using MAHAK.Integration.Api.Security;
using MAHAK.Integration.Api.Services;

namespace MAHAK.Integration.Api.Endpoints;

public static class ItemEndpoints
{
    public static RouteGroupBuilder MapItemEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/items/sync", async (
            ItemSyncRequest request,
            HttpContext context,
            IBrandContextAccessor brands,
            ItemService service,
            CancellationToken cancellationToken) =>
                Results.Ok(await service.SyncAsync(
                    brands.GetRequired(context),
                    request,
                    cancellationToken)))
            .WithName("SyncMahakItems")
            .WithSummary("Synchronizes the explicitly selected MAHAK item projection.");
        return group;
    }
}
