using FluentValidation;

namespace InvoiceClean.Application.Invoices.RemoveInvoiceLine;

public sealed class RemoveInvoiceLineValidator : AbstractValidator<RemoveInvoiceLineCommand>
{
    public RemoveInvoiceLineValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Invoice ID is required.");

        RuleFor(x => x.LineId)
            .NotEmpty()
            .WithMessage("Line ID is required.");
    }
}
