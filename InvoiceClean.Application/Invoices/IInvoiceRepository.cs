using InvoiceClean.Domain.Invoices;

namespace InvoiceClean.Application.Invoices
{
    public interface IInvoiceRepository
    {
        Task AddAsync(Invoice invoice, CancellationToken cancellationToken);
        Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Invoice>> GetAllAsync(CancellationToken cancellationToken);
    }
}
