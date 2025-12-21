using InvoiceClean.Application.Common.Results;
using InvoiceClean.Application.Invoices.GetInvoiceById;
using MediatR;

namespace InvoiceClean.Application.Invoices.UpdateInvoice
{
    public sealed class UpdateInvoiceHandler : IRequestHandler<UpdateInvoiceCommand, Result<InvoiceDto>>
    {
        private readonly IInvoiceRepository _repo;
        public UpdateInvoiceHandler(IInvoiceRepository repo) => _repo = repo;

        public async Task<Result<InvoiceDto>> Handle(UpdateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _repo.GetByIdAsync(request.Id, cancellationToken);

            if (invoice is null)
            {
                return Result<InvoiceDto>.Fail(new Error(
                    ErrorType.NotFound,
                    Code: "invoice_not_found",
                    Message: $"Invoice '{request.Id}' was not found."
                ));
            }

            invoice.UpdateNumber(request.Number);
            invoice.UpdateDate(request.Date);
            invoice.UpdateCustomer(request.CustomerName, request.CustomerAddress, request.CustomerVat);

            await _repo.UpdateAsync(invoice, cancellationToken);

            var lines = invoice.Lines
                .Select(l => new InvoiceLineDto(l.Id, l.Description, l.Quantity, l.UnitPrice, l.LineTotal))
                .ToList();

            var dto = new InvoiceDto(invoice.Id, invoice.Number, invoice.Date, invoice.Total, lines);

            return Result<InvoiceDto>.Ok(dto);
        }
    }
}
