using InvoiceClean.Application.Common.Results;
using InvoiceClean.Application.Invoices.GetInvoiceById;
using MediatR;

namespace InvoiceClean.Application.Invoices.UpdateInvoice
{
    public sealed record UpdateInvoiceCommand(
    Guid Id,
    string Number,
    DateOnly Date,
    string CustomerName,
    string? CustomerAddress,
    string? CustomerVat) : IRequest<Result<InvoiceDto>>;
}
