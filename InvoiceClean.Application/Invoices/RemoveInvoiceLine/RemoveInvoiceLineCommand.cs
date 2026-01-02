using InvoiceClean.Application.Common.Results;
using InvoiceClean.Application.Invoices.GetInvoiceById;
using MediatR;

namespace InvoiceClean.Application.Invoices.RemoveInvoiceLine;

public sealed record RemoveInvoiceLineCommand(
    Guid InvoiceId,
    Guid LineId
) : IRequest<Result<InvoiceDto>>;