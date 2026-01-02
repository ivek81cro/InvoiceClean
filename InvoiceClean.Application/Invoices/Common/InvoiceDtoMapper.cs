using InvoiceClean.Application.Invoices.GetInvoiceById;
using InvoiceClean.Domain.Invoices;

namespace InvoiceClean.Application.Invoices.Common;

internal static class InvoiceDtoMapper
{
    public static InvoiceDto ToDto(this Invoice invoice)
    {
        var lines = invoice.Lines
            .Select(l => new InvoiceLineDto(l.Id, l.Description, l.Quantity, l.UnitPrice, l.LineTotal))
            .ToList();

        return new InvoiceDto(invoice.Id, invoice.Number, invoice.Date, invoice.Total, lines);
    }
}
