using Kahrad.Pricing.AccountingIntegration.Application.Contracts;

namespace Kahrad.Pricing.Api.IntegrationTests.Setup;

public sealed class RecordingAccountingProvider : IAccountingProviderClient
{
    public bool IsConfigured { get; set; }
    public AccountingSnapshot<ImportedAccountingItem> Items { get; set; } = new([], true);
    public AccountingSnapshot<ImportedAccountingRepresentative> Representatives { get; set; } = new([], true);
    public AccountingSnapshot<ImportedAccountingInvoice> Invoices { get; set; } = new([], true);
    public List<AccountingConnectionContext> Calls { get; } = [];

    public void Reset()
    {
        IsConfigured = false;
        Items = new([], true); Representatives = new([], true); Invoices = new([], true);
        Calls.Clear();
    }

    public Task<AccountingSnapshot<ImportedAccountingItem>> ReadItemsAsync(AccountingConnectionContext connection, CancellationToken cancellationToken)
    {
        Verify(connection); return Task.FromResult(Items);
    }
    public Task<AccountingSnapshot<ImportedAccountingRepresentative>> ReadRepresentativesAsync(AccountingConnectionContext connection, CancellationToken cancellationToken)
    {
        Verify(connection); return Task.FromResult(Representatives);
    }
    public Task<AccountingSnapshot<ImportedAccountingInvoice>> ReadInvoicesAsync(AccountingConnectionContext connection, CancellationToken cancellationToken)
    {
        Verify(connection); return Task.FromResult(Invoices);
    }
    private void Verify(AccountingConnectionContext context)
    {
        if (!IsConfigured) throw new AccountingProviderNotConfiguredException();
        Calls.Add(context);
    }
}
