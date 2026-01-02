using InvoiceClean.Application.Common.Results;
using InvoiceClean.Application.Invoices.GetInvoiceById;
using MediatR;

namespace InvoiceClean.Application.Invoices.UpdateInvoiceLine;

public sealed record UpdateInvoiceLineCommand(
    Guid InvoiceId,
    Guid LineId,
    string Description,
    decimal Quantity,
    decimal UnitPrice
) : IRequest<Result<InvoiceDto>>;