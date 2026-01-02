using InvoiceClean.Application.Common.Results;
using InvoiceClean.Application.Invoices.GetInvoiceById;
using InvoiceClean.Domain;
using InvoiceClean.Domain.Invoices;

namespace InvoiceClean.Application.Invoices.Common;

public abstract class InvoiceLineOperationHandler
{
    private readonly IInvoiceRepository _repo;

    protected InvoiceLineOperationHandler(IInvoiceRepository repo) => _repo = repo;

    protected async Task<Result<InvoiceDto>> ExecuteAsync(
        Guid invoiceId,
        Action<Invoice> operation,
        CancellationToken cancellationToken)
    {
        var invoice = await _repo.GetByIdAsync(invoiceId, cancellationToken);

        if (invoice is null)
        {
            return Result<InvoiceDto>.Fail(new Error(
                ErrorType.NotFound,
                Code: InvoiceErrors.NotFoundCode,
                Message: InvoiceErrors.NotFoundMessage(invoiceId)
            ));
        }

        try
        {
            operation(invoice);
            await _repo.UpdateAsync(invoice, cancellationToken);

            return Result<InvoiceDto>.Ok(invoice.ToDto());
        }
        catch (DomainException ex)
        {
            return Result<InvoiceDto>.Fail(new Error(
                ErrorType.Validation,
                Code: InvoiceErrors.DomainValidationCode,
                Message: ex.Message
            ));
        }
    }
}
