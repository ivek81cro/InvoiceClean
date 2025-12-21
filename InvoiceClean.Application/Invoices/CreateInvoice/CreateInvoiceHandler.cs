using InvoiceClean.Domain.Invoices;
using MediatR;

namespace InvoiceClean.Application.Invoices.CreateInvoice
{
    public sealed class CreateInvoiceHandler : IRequestHandler<CreateInvoiceCommand, Guid>
    {
        private readonly IInvoiceRepository _repo;
        public CreateInvoiceHandler(IInvoiceRepository repo) => _repo = repo;

        public async Task<Guid> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = new Invoice(request.Number, request.Date);

            foreach (var l in request.Lines)
                invoice.AddLine(l.Description, l.Quantity, l.UnitPrice);

            await _repo.AddAsync(invoice, cancellationToken);
            return invoice.Id;
        }
    }
}
