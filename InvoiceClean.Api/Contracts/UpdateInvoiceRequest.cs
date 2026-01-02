namespace InvoiceClean.Api.Contracts;

public sealed record UpdateInvoiceRequest(
    string Number,
    DateOnly Date,
    string CustomerName,
    string? CustomerAddress,
    string? CustomerVat
);