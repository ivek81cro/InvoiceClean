using InvoiceClean.Application.Common.Results;
using InvoiceClean.Application.Invoices.GetInvoiceById;
using MediatR;

namespace InvoiceClean.Application.Invoices.UpdateInvoiceLine;

public sealed class UpdateInvoiceLineHandler : IRequestHandler<UpdateInvoiceLineCommand, Result<InvoiceDto>>
{
    private readonly IInvoiceRepository _repo;
    
    public UpdateInvoiceLineHandler(IInvoiceRepository repo) => _repo = repo;

    public async Task<Result<InvoiceDto>> Handle(UpdateInvoiceLineCommand request, CancellationToken cancellationToken)
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

        invoice.UpdateLine(request.LineId, request.Description, request.Quantity, request.UnitPrice);
        await _repo.UpdateAsync(invoice, cancellationToken);

        var lines = invoice.Lines
            .Select(l => new InvoiceLineDto(l.Id, l.Description, l.Quantity, l.UnitPrice, l.LineTotal))
            .ToList();

        var dto = new InvoiceDto(invoice.Id, invoice.Number, invoice.Date, invoice.Total, lines);
        return Result<InvoiceDto>.Ok(dto);
    }
}