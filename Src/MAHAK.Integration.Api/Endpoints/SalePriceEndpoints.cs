using MAHAK.Integration.Api.Contracts.Prices;
using MAHAK.Integration.Api.Security;
using MAHAK.Integration.Api.Services;

namespace MAHAK.Integration.Api.Endpoints;

public static class SalePriceEndpoints
{
    public static RouteGroupBuilder MapSalePriceEndpoint(this RouteGroupBuilder group)
    {
        group.MapPost("/sales/prices", async (
            SetSalePriceRequest request,
            HttpContext context,
            IBrandContextAccessor brands,
            SalePriceService service,
            CancellationToken cancellationToken) =>
                Results.Ok(await service.UpdateAsync(
                    brands.GetRequired(context),
                    request,
                    cancellationToken)))
            .WithName("SetMahakConsumerPrices")
            .WithSummary("Sets the MAHAK current IRR and USD consumer prices supplied by the caller.");
        return group;
    }
}
