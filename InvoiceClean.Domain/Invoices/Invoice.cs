namespace InvoiceClean.Domain.Invoices
{
    public sealed class Invoice
    {
        private readonly List<InvoiceLine> _lines = new();

        public Guid Id { get; private set; } = Guid.NewGuid();
        public string Number { get; private set; } = default!;
        public DateOnly Date { get; private set; }
        public string CustomerName { get; private set; } = string.Empty;
        public string? CustomerAddress { get; private set; }
        public string? CustomerVat { get; private set; }
        public IReadOnlyCollection<InvoiceLine> Lines => _lines;

        private Invoice() { }

        public Invoice(string number, DateOnly date)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new DomainException("Invoice number is required.");

            Number = number;
            Date = date;
        }

        public void AddLine(string description, decimal qty, decimal unitPrice)
        {
            _lines.Add(new InvoiceLine(description, qty, unitPrice));
        }

        public decimal Total => _lines.Sum(l => l.LineTotal);

        public void UpdateNumber(string number)
        {
            Number = number;
        }

        public void UpdateDate(DateOnly date)
        {
            Date = date;
        }

        public void UpdateCustomer(string customerName, string? customerAddress, string? customerVat)
        {
            CustomerName = customerName;
            CustomerAddress = customerAddress;
            CustomerVat = customerVat;
        }

        public void UpdateLine(Guid lineId, string description, decimal quantity, decimal unitPrice)
        {
            var line = _lines.FirstOrDefault(l => l.Id == lineId);
            if (line is null)
                throw new DomainException($"Invoice line with ID '{lineId}' not found.");

            line.Update(description, quantity, unitPrice);
        }

        public void RemoveLine(Guid lineId)
        {
            var line = _lines.FirstOrDefault(l => l.Id == lineId);
            if (line is null)
                throw new DomainException($"Invoice line with ID '{lineId}' not found.");

            if (_lines.Count == 1)
                throw new DomainException("Cannot remove the last invoice line. Invoice must have at least one line.");

            _lines.Remove(line);
        }
    }
}
