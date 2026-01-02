using InvoiceClean.Application.Common.Results;
using InvoiceClean.Application.Invoices.GetInvoiceById;
using MediatR;

namespace InvoiceClean.Application.Invoices.RemoveInvoiceLine;

public sealed class RemoveInvoiceLineHandler : IRequestHandler<RemoveInvoiceLineCommand, Result<InvoiceDto>>
{
    private readonly IInvoiceRepository _repo;
    
    public RemoveInvoiceLineHandler(IInvoiceRepository repo) => _repo = repo;

    public async Task<Result<InvoiceDto>> Handle(RemoveInvoiceLineCommand request, CancellationToken cancellationToken)
    {
        var invoice = await _repo.GetByIdAsync(request.InvoiceId, cancellationToken);
        
        if (invoice is null)
        {
            return Result<InvoiceDto>.Fail(new Error(
                ErrorType.NotFound,
                Code: "invoice_not_found",
                Message: $"Invoice '{request.InvoiceId}' was not found."
            ));
        }

        invoice.RemoveLine(request.LineId);
        await _repo.UpdateAsync(invoice, cancellationToken);

        var lines = invoice.Lines
            .Select(l => new InvoiceLineDto(l.Id, l.Description, l.Quantity, l.UnitPrice, l.LineTotal))
            .ToList();

        var dto = new InvoiceDto(invoice.Id, invoice.Number, invoice.Date, invoice.Total, lines);
        return Result<InvoiceDto>.Ok(dto);
    }
}