using Kahrad.Pricing.AccountingIntegration.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Kahrad.Pricing.AccountingIntegration.Infrastructure.Persistence;

public sealed class AccountingInvoiceReader(AccountingDbContext db) : IAccountingInvoiceReader
{
    public async Task<IReadOnlyList<AccountingInvoiceView>> SearchAsync(long brandId, string? search,
        int page, int pageSize, CancellationToken ct)
    {
        var query = db.Invoices.AsNoTracking().Where(x => x.BrandId == brandId && !x.IsDeleted);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.ExternalInvoiceId.Contains(search) ||
                (x.InvoiceNumber != null && x.InvoiceNumber.Contains(search)));
        return await query.OrderByDescending(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new AccountingInvoiceView(x.Id, x.BrandId, x.ExternalInvoiceId,
                x.ExternalRepresentativeId, x.InvoiceNumber, x.ExternalStatus,
                x.IssuedAtUtc, x.LastSeenAtUtc)).ToListAsync(ct);
    }
}
