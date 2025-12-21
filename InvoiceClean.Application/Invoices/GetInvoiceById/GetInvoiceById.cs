using InvoiceClean.Application.Common.Results;
using MediatR;

namespace InvoiceClean.Application.Invoices.GetInvoiceById
{
    public sealed record GetInvoiceByIdQuery(Guid Id) : IRequest<Result<InvoiceDto>>;

    public sealed record InvoiceDto(
        Guid Id,
        string Number,
        DateOnly Date,
        decimal Total,
        IReadOnlyList<InvoiceLineDto> Lines
    );
    public sealed record InvoiceLineDto(
        Guid Id,
        string Description,
        decimal Quantity,
        decimal UnitPrice,
        decimal LineTotal
     );
}
