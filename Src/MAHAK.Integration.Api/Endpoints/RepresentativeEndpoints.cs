using MAHAK.Integration.Api.Contracts.Common;
using MAHAK.Integration.Api.Security;
using MAHAK.Integration.Api.Services;

namespace MAHAK.Integration.Api.Endpoints;

public static class RepresentativeEndpoints
{
    public static RouteGroupBuilder MapRepresentativeEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/representatives/sync", async (
            SyncRequest request,
            HttpContext context,
            IBrandContextAccessor brands,
            RepresentativeService service,
            CancellationToken cancellationToken) =>
                Results.Ok(await service.SyncAsync(
                    brands.GetRequired(context),
                    request,
                    cancellationToken)))
            .WithName("SyncMahakRepresentatives")
            .WithSummary("Synchronizes the bounded MAHAK representative projection.");
        return group;
    }
}
