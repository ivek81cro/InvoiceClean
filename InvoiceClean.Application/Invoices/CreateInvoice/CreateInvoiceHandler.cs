using InvoiceClean.Application.Common.Results;
using InvoiceClean.Domain.Invoices;
using MediatR;

namespace InvoiceClean.Application.Invoices.CreateInvoice
{
    public sealed class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, Result<Guid>>
    {
        private readonly IInvoiceRepository _repo;
        public CreateInvoiceHandler(IInvoiceRepository repo) => _repo = repo;

        public async Task<Result<Guid>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = new Invoice(request.Number, request.Date);

            foreach (var l in request.Lines)
                invoice.AddLine(l.Description, l.Quantity, l.UnitPrice);

            await _repo.AddAsync(invoice, cancellationToken);
            return Result<Guid>.Ok(invoice.Id);
        }
    }
}
