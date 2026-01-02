namespace InvoiceClean.Api.Contracts;

public sealed record AddInvoiceLineRequest(
    string Description,
    decimal Quantity,
    decimal UnitPrice
);