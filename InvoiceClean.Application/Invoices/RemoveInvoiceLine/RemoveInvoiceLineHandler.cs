using InvoiceClean.Application.Common.Results;
using InvoiceClean.Application.Invoices.Common;
using InvoiceClean.Application.Invoices.GetInvoiceById;
using MediatR;

namespace InvoiceClean.Application.Invoices.RemoveInvoiceLine;

public sealed class RemoveInvoiceLineHandler : InvoiceLineOperationHandler, IRequestHandler<RemoveInvoiceLineCommand, Result<InvoiceDto>>
{
    public RemoveInvoiceLineHandler(IInvoiceRepository repo) : base(repo) { }

    public Task<Result<InvoiceDto>> Handle(RemoveInvoiceLineCommand request, CancellationToken cancellationToken)
    {
        return ExecuteAsync(
            request.InvoiceId,
            invoice => invoice.RemoveLine(request.LineId),
            cancellationToken
        );
    }
}