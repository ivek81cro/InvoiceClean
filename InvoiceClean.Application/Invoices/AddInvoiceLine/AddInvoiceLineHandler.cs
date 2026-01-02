using InvoiceClean.Application.Common.Results;
using InvoiceClean.Application.Invoices.Common;
using InvoiceClean.Application.Invoices.GetInvoiceById;
using MediatR;

namespace InvoiceClean.Application.Invoices.AddInvoiceLine;

public sealed class AddInvoiceLineHandler : InvoiceLineOperationHandler, IRequestHandler<AddInvoiceLineCommand, Result<InvoiceDto>>
{
    public AddInvoiceLineHandler(IInvoiceRepository repo) : base(repo) { }

    public Task<Result<InvoiceDto>> Handle(AddInvoiceLineCommand request, CancellationToken cancellationToken)
    {
        return ExecuteAsync(
            request.InvoiceId,
            invoice => invoice.AddLine(request.Description, request.Quantity, request.UnitPrice),
            cancellationToken
        );
    }
}