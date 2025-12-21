using InvoiceClean.Application.Common.Results;
using MediatR;

namespace InvoiceClean.Application.Invoices.GetInvoiceById
{
    internal class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
    {
        private readonly IInvoiceRepository _repo;

        public GetInvoiceByIdHandler(IInvoiceRepository repo) => _repo = repo;

        public async Task<Result<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var inv = await _repo.GetByIdAsync(request.Id, cancellationToken);
            if (inv is null)
            {
                return Result<InvoiceDto>.Fail(new Error(
                ErrorType.NotFound,
                Code: "invoice_not_found",
                Message: $"Invoice '{request.Id}' was not found."
            ));
            }

            var lines = inv.Lines
                .Select(l => new InvoiceLineDto(l.Id, l.Description, l.Quantity, l.UnitPrice, l.LineTotal))
                .ToList();

            var dto = new InvoiceDto(inv.Id, inv.Number, inv.Date, inv.Total, lines);
            return Result<InvoiceDto>.Ok(dto);
        }
    }
}
