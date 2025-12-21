using FluentValidation;

namespace InvoiceClean.Application.Invoices.CreateInvoice
{
    public sealed class CreateInvoiceValidator : AbstractValidator<CreateInvoiceCommand>
    {
        public CreateInvoiceValidator() 
        {
            RuleFor(x => x.Number).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Lines).NotEmpty();

            RuleForEach(x => x.Lines).ChildRules(line =>
            {
                line.RuleFor(l => l.Description).NotEmpty().MaximumLength(200);
                line.RuleFor(l => l.Quantity).GreaterThan(0);
                line.RuleFor(l => l.UnitPrice).GreaterThanOrEqualTo(0);
            });
        }
    }
}
