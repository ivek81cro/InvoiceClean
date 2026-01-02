using InvoiceClean.Application.Common.Results;
using InvoiceClean.Application.Invoices.Common;
using MediatR;

namespace InvoiceClean.Application.Invoices.GetInvoiceById
{
    internal sealed class GetInvoiceByIdHandler : IRequestHandler<GetInvoiceByIdQuery, Result<InvoiceDto>>
    {
        private readonly IInvoiceRepository _repo;

        public GetInvoiceByIdHandler(IInvoiceRepository repo) => _repo = repo;

        public async Task<Result<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
        {
            var invoice = await _repo.GetByIdAsync(request.Id, cancellationToken);
            
            if (invoice is null)
            {
                return Result<InvoiceDto>.Fail(new Error(
                    ErrorType.NotFound,
                    Code: InvoiceErrors.NotFoundCode,
                    Message: InvoiceErrors.NotFoundMessage(request.Id)
                ));
            }

            return Result<InvoiceDto>.Ok(invoice.ToDto());
        }
    }
}
