using InvoiceClean.Domain.Invoices;
using Microsoft.EntityFrameworkCore;

namespace InvoiceClean.Infrastructure.Persistence
{
    public sealed class AppDbContext : DbContext
    {
        public DbSet<Invoice> Invoices => Set<Invoice>();

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Invoice>(b =>
            {
                b.ToTable("Invoices");
                b.HasKey(x => x.Id);

                b.Property(x => x.Number).HasMaxLength(50).IsRequired();
                b.Property(x => x.CustomerName).HasMaxLength(200).IsRequired();
                b.Property(x => x.CustomerAddress).HasMaxLength(500);
                b.Property(x => x.CustomerVat).HasMaxLength(50);

                // Mapiraj navigaciju preko property-ja Lines
                b.HasMany(x => x.Lines)
                 .WithOne()
                 .OnDelete(DeleteBehavior.Cascade);

                // Reci EF-u da Lines koristi backing field "_lines"
                b.Navigation(x => x.Lines)
                 .HasField("_lines")
                 .UsePropertyAccessMode(PropertyAccessMode.Field);
            });

            modelBuilder.Entity<InvoiceLine>(b =>
            {
                b.ToTable("InvoiceLines");
                b.HasKey(x => x.Id);
                b.Property(x => x.Description).HasMaxLength(200).IsRequired();
            });
        }
    }
}
