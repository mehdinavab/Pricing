using System.Diagnostics;
using MAHAK.Integration.Api.Contracts.Prices;
using MAHAK.Integration.Api.Data.Repositories;
using MAHAK.Integration.Api.Errors;
using MAHAK.Integration.Api.Security;

namespace MAHAK.Integration.Api.Services;

public sealed class SalePriceService(
    ISalePriceRepository repository,
    ILogger<SalePriceService> logger)
{
    public async Task<SetSalePriceResponse> UpdateAsync(
        BrandContext brand,
        SetSalePriceRequest request,
        CancellationToken cancellationToken)
    {
        Validate(request);
        var timer = Stopwatch.StartNew();
        var result = await repository.UpdateAsync(brand, request, cancellationToken);
        logger.LogInformation(
            "MAHAK sale-price update completed for brand {BrandCode} with outcome {Outcome} in {ElapsedMilliseconds} ms",
            brand.BrandCode,
            result,
            timer.ElapsedMilliseconds);
        return result switch
        {
            SalePriceUpdateResult.Updated => new SetSalePriceResponse(request.GoodCode, true, 1),
            SalePriceUpdateResult.NotFound => throw new AccountingRecordNotFoundException(
                "No matching MAHAK product price record was found."),
            SalePriceUpdateResult.MultipleMatches => throw new AccountingDataIntegrityException(
                "Multiple MAHAK product price records matched; the update was rolled back."),
            _ => throw new InvalidOperationException("Unknown sale-price update outcome.")
        };
    }

    public static void Validate(SetSalePriceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.GoodCode) || request.GoodCode.Length > 128 ||
            request.GoodCode.Any(char.IsControl))
            throw new RequestValidationException("GoodCode must contain 1 to 128 non-control characters.");
        if (request.ConsumerPrice < 0 || request.ConsumerPriceDollar < 0)
            throw new RequestValidationException("Consumer prices cannot be negative.");
        if (request.OperationId == Guid.Empty)
            throw new RequestValidationException("OperationId is required.");
    }
}
