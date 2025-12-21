namespace InvoiceClean.Domain.Invoices
{
    public sealed class InvoiceLine
    {
        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Description { get; private set; } = default!;
        public decimal Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }

        private InvoiceLine() { }

        public InvoiceLine(string description, decimal quantity, decimal unitPrice) 
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new DomainException("Description is required.");
            if (quantity <= 0) throw new DomainException("Quantity must be > 0.");
            if (unitPrice < 0) throw new DomainException("Unit price must be >= 0.");

            Description = description;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }

        public decimal LineTotal => Quantity * UnitPrice;
    }
}
