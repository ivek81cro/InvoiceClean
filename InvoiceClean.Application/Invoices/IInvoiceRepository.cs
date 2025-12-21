using InvoiceClean.Domain.Invoices;

namespace InvoiceClean.Application.Invoices
{
    public interface IInvoiceRepository
    {
        Task AddAsync(Invoice invoice, CancellationToken ct);
        Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct);
    }
}
