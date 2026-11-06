using MAHAK.Integration.Api.Contracts.Common;
using MAHAK.Integration.Api.Security;
using MAHAK.Integration.Api.Services;

namespace MAHAK.Integration.Api.Endpoints;

public static class InvoiceEndpoints
{
    public static RouteGroupBuilder MapInvoiceEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/invoices/sync", async (
            SyncRequest request,
            HttpContext context,
            IBrandContextAccessor brands,
            InvoiceService service,
            CancellationToken cancellationToken) =>
                Results.Ok(await service.SyncAsync(
                    brands.GetRequired(context),
                    request,
                    cancellationToken)))
            .WithName("SyncMahakInvoices")
            .WithSummary("Synchronizes the bounded legacy non-returned invoice projection.");
        return group;
    }
}
