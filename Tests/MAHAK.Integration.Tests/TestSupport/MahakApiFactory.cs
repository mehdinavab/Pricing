using MAHAK.Integration.Api.Contracts.Items;
using MAHAK.Integration.Api.Data.Repositories;
using MAHAK.Integration.Api.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace MAHAK.Integration.Tests.TestSupport;

internal sealed class MahakApiFactory : WebApplicationFactory<Program>
{
    public RecordingItemRepository Items { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
        builder.ConfigureAppConfiguration(configuration =>
            configuration.AddInMemoryCollection(MahakTestData.Configuration()));
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IItemRepository>();
            services.AddSingleton<IItemRepository>(Items);
        });
    }
}

internal sealed class RecordingItemRepository : IItemRepository
{
    private readonly List<BrandContext> calls = [];
    public IReadOnlyList<BrandContext> Calls => calls;

    public Task<IReadOnlyList<AccountingItem>> GetReturnedItemsAsync(
        BrandContext brand,
        int take,
        CancellationToken cancellationToken)
    {
        lock (calls)
            calls.Add(brand);
        return Task.FromResult<IReadOnlyList<AccountingItem>>([]);
    }
}
