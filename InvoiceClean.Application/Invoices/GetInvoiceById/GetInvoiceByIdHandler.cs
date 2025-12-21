using MediatR;

namespace InvoiceClean.Application.Invoices.GetInvoiceById
{
    internal class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto?>
    {
        private readonly IInvoiceRepository _repo;

        public GetInvoiceByIdHandler(IInvoiceRepository repo) => _repo = repo;

        public async Task<InvoiceDto?> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var inv = await _repo.GetByIdAsync(request.Id, cancellationToken);
            if (inv is null) return null;

            var lines = inv.Lines
                .Select(l => new InvoiceLineDto(
                    l.Id,
                    l.Description,
                    l.Quantity,
                    l.UnitPrice,
                    l.LineTotal))
                .ToList();

            return new InvoiceDto(
                inv.Id,
                inv.Number,
                inv.Date,
                inv.Total,
                lines
            );
        }
    }
}
