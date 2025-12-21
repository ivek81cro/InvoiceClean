using InvoiceClean.Application.Common.Results;
using MediatR;

namespace InvoiceClean.Application.Invoices.GetAllInvoiceId
{
    public sealed record GetAllInvoiceIdsQuery : IRequest<Result<List<InvoiceIdDto>>>;

    public sealed record InvoiceIdDto(Guid Id, string Number, DateOnly Date);
}
