using MediatR;

namespace InvoiceClean.Application.Invoices.CreateInvoice
{
    public sealed record CreateInvoiceCommand(
        string Number,
        DateOnly Date,
        IReadOnlyList<CreateInvoiceLineDto> Lines) : IRequest<Guid>;

    public sealed record CreateInvoiceLineDto(string Description, decimal Quantity, decimal UnitPrice);
}
