using InvoiceClean.Application.Common.Results;
using MediatR;

namespace InvoiceClean.Application.Invoices.GetAllInvoiceId
{
    public sealed class GetAllInvoiceIdsHandler : IRequestHandler<GetAllInvoiceIdsQuery, Result<List<InvoiceIdDto>>>
    {
        private readonly IInvoiceRepository _repo;
        public GetAllInvoiceIdsHandler(IInvoiceRepository repo) => _repo = repo;

        public async Task<Result<List<InvoiceIdDto>>> Handle(GetAllInvoiceIdsQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _repo.GetAllAsync(cancellationToken);

            var dtos = invoices
                .Select(inv => new InvoiceIdDto(inv.Id, inv.Number, inv.Date))
                .ToList();

            return Result<List<InvoiceIdDto>>.Ok(dtos);
        }
    }
}
