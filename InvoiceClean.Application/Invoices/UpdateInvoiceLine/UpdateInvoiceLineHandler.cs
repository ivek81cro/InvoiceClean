using InvoiceClean.Application.Common.Results;
using InvoiceClean.Application.Invoices.Common;
using InvoiceClean.Application.Invoices.GetInvoiceById;
using MediatR;

namespace InvoiceClean.Application.Invoices.UpdateInvoiceLine;

public sealed class UpdateInvoiceLineHandler : InvoiceLineOperationHandler, IRequestHandler<UpdateInvoiceLineCommand, Result<InvoiceDto>>
{
    public UpdateInvoiceLineHandler(IInvoiceRepository repo) : base(repo) { }

    public Task<Result<InvoiceDto>> Handle(UpdateInvoiceLineCommand request, CancellationToken cancellationToken)
    {
        return ExecuteAsync(
            request.InvoiceId,
            invoice => invoice.UpdateLine(request.LineId, request.Description, request.Quantity, request.UnitPrice),
            cancellationToken
        );
    }
}