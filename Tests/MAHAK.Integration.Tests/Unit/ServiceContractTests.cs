using MAHAK.Integration.Api.Configuration;
using MAHAK.Integration.Api.Contracts.Common;
using MAHAK.Integration.Api.Contracts.Prices;
using MAHAK.Integration.Api.Data.Repositories;
using MAHAK.Integration.Api.Errors;
using MAHAK.Integration.Api.Security;
using MAHAK.Integration.Api.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace MAHAK.Integration.Tests.Unit;

public sealed class ServiceContractTests
{
    private static readonly BrandContext Brand = new(
        "Kahrad", "sql", "db", "IntegratedSecurity", null, null,
        true, false, 5, 10, 30);

    [Fact]
    public void Cursor_is_blocked_until_a_real_unique_key_is_confirmed()
    {
        var policy = new BoundedSyncPolicy(Options.Create(new MahakOptions()));
        Assert.Throws<SchemaConfirmationRequiredException>(() =>
            policy.Validate(new SyncRequest { Cursor = "invented" }));
    }

    [Fact]
    public void Bounded_sync_rejects_a_truncated_result_instead_of_claiming_completion()
    {
        Assert.Throws<SchemaConfirmationRequiredException>(() =>
            BoundedSyncPolicy.Complete(new[] { 1, 2 }, 1));
    }

    [Theory]
    [InlineData(-1, 1)]
    [InlineData(1, -1)]
    public void Price_validation_rejects_negative_values(decimal irr, decimal usd)
    {
        Assert.Throws<RequestValidationException>(() => SalePriceService.Validate(new SetSalePriceRequest
        {
            GoodCode = "12050",
            ConsumerPrice = irr,
            ConsumerPriceDollar = usd,
            OperationId = Guid.NewGuid()
        }));
    }

    [Fact]
    public async Task Price_service_returns_one_updated_row_without_currency_conversion()
    {
        var repository = new PriceRepository(SalePriceUpdateResult.Updated);
        var service = new SalePriceService(repository, NullLogger<SalePriceService>.Instance);
        var request = new SetSalePriceRequest
        {
            GoodCode = "12050",
            ConsumerPrice = 150_000_000m,
            ConsumerPriceDollar = 100.25m,
            OperationId = Guid.NewGuid()
        };

        var response = await service.UpdateAsync(Brand, request, CancellationToken.None);

        Assert.True(response.Updated);
        Assert.Equal(1, response.AffectedRows);
        Assert.Same(request, repository.Request);
        Assert.Equal(100.25m, repository.Request!.ConsumerPriceDollar);
    }

    [Fact]
    public async Task Multiple_price_rows_are_reported_as_integrity_conflict()
    {
        var service = new SalePriceService(
            new PriceRepository(SalePriceUpdateResult.MultipleMatches),
            NullLogger<SalePriceService>.Instance);
        var request = new SetSalePriceRequest
        {
            GoodCode = "12050",
            ConsumerPrice = 1,
            ConsumerPriceDollar = 1,
            OperationId = Guid.NewGuid()
        };

        await Assert.ThrowsAsync<AccountingDataIntegrityException>(() =>
            service.UpdateAsync(Brand, request, CancellationToken.None));
    }

    private sealed class PriceRepository(SalePriceUpdateResult result) : ISalePriceRepository
    {
        public SetSalePriceRequest? Request { get; private set; }

        public Task<SalePriceUpdateResult> UpdateAsync(
            BrandContext brand,
            SetSalePriceRequest request,
            CancellationToken cancellationToken)
        {
            Request = request;
            return Task.FromResult(result);
        }
    }
}
