namespace InvoiceClean.Application.Invoices.Common;

internal static class InvoiceErrors
{
    public const string NotFoundCode = "invoice_not_found";
    public const string DomainValidationCode = "domain_validation_error";
    
    public static string NotFoundMessage(Guid invoiceId) => 
        $"Invoice '{invoiceId}' was not found.";
}
