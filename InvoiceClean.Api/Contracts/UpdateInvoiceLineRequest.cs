namespace InvoiceClean.Api.Contracts;

public sealed record UpdateInvoiceLineRequest(
    string Description,
    decimal Quantity,
    decimal UnitPrice
);