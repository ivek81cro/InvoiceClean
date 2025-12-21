using InvoiceClean.Application.Invoices;
using InvoiceClean.Domain.Invoices;
using InvoiceClean.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace InvoiceClean.Infrastructure.Invoices
{
    public sealed class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _db;

        public InvoiceRepository(AppDbContext db)=> _db = db;

        public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken)
        {
            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync(cancellationToken);
        }

        public Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return _db.Invoices.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<Invoice>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _db.Invoices
                .OrderByDescending(x => x.Date)
                .ToListAsync(cancellationToken);
        }
    }
}
