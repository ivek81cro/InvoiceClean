using FluentValidation;

namespace InvoiceClean.Application.Invoices.UpdateInvoice
{
    public sealed class UpdateInvoiceValidator : AbstractValidator<UpdateInvoiceCommand>
    {
        public UpdateInvoiceValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Invoice ID is required.");

            RuleFor(x => x.Number)
                .NotEmpty()
                .WithMessage("Invoice number is required.")
                .MaximumLength(50)
                .WithMessage("Invoice number cannot exceed 50 characters.");

            RuleFor(x => x.Date)
                .NotEmpty()
                .WithMessage("Invoice date is required.")
                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today.AddDays(30)))
                .WithMessage("Invoice date cannot be more than 30 days in the future.");

            RuleFor(x => x.CustomerName)
                .NotEmpty()
                .WithMessage("Customer name is required.")
                .MaximumLength(200)
                .WithMessage("Customer name cannot exceed 200 characters.");

            RuleFor(x => x.CustomerAddress)
                .MaximumLength(500)
                .WithMessage("Customer address cannot exceed 500 characters.")
                .When(x => !string.IsNullOrEmpty(x.CustomerAddress));

            RuleFor(x => x.CustomerVat)
                .MaximumLength(50)
                .WithMessage("Customer VAT cannot exceed 50 characters.")
                .When(x => !string.IsNullOrEmpty(x.CustomerVat));
        }
    }
}
