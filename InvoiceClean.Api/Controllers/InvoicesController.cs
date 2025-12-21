using InvoiceClean.Api.Common;
using InvoiceClean.Application.Invoices.CreateInvoice;
using InvoiceClean.Application.Invoices.GetInvoiceById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceClean.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class InvoicesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InvoicesController(IMediator mediator) => _mediator = mediator;

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateInvoiceRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(
                new CreateInvoiceCommand(request.Number, request.Date, request.Lines), cancellationToken);

            return this.CreatedFromResult(nameof(GetById), new { id = result.Value }, result);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetInvoiceByIdQuery(id), cancellationToken);
            return this.ToActionResult(result);
        }
    }

    public sealed record CreateInvoiceRequest(
        string Number,
        DateOnly Date,
        List<CreateInvoiceLineDto> Lines
    );
}
