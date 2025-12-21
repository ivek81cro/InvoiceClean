using InvoiceClean.Application.Invoices;
using InvoiceClean.Domain.Invoices;
using InvoiceClean.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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

        public Task<Invoice?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return _db.Invoices.Include("_lines").FirstOrDefaultAsync(x => x.Id == id, ct);
        }
    }
}
