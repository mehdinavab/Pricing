namespace MAHAK.Integration.Api.Contracts.Invoices;

public sealed record CurrentInvoiceLine(
    string? Fax,
    string PersonCode,
    string ProductCode,
    string InvoiceCode);
