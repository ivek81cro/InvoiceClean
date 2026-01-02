using InvoiceClean.Application.Common.Results;
using InvoiceClean.Application.Invoices.GetInvoiceById;
using MediatR;

namespace InvoiceClean.Application.Invoices.AddInvoiceLine;

public sealed record AddInvoiceLineCommand(
    Guid InvoiceId,
    string Description,
    decimal Quantity,
    decimal UnitPrice
) : IRequest<Result<InvoiceDto>>;