using FluentValidation;

namespace InvoiceClean.Application.Invoices.UpdateInvoiceLine;

public sealed class UpdateInvoiceLineValidator : AbstractValidator<UpdateInvoiceLineCommand>
{
    public UpdateInvoiceLineValidator()
    {
        RuleFor(x => x.InvoiceId)
            .NotEmpty()
            .WithMessage("Invoice ID is required.");

        RuleFor(x => x.LineId)
            .NotEmpty()
            .WithMessage("Line ID is required.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(200)
            .WithMessage("Description cannot exceed 200 characters.");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");

        RuleFor(x => x.UnitPrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Unit price must be greater than or equal to 0.");
    }
}
