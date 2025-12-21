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
            var id = await _mediator.Send(
                new CreateInvoiceCommand(request.Number, request.Date, request.Lines), cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var dto = await _mediator.Send(new GetInvoiceByIdQuery(id), cancellationToken);
            if (dto is null) return NotFound();

            return Ok(dto);
        }
    }

    public sealed record CreateInvoiceRequest(
        string Number,
        DateOnly Date,
        List<CreateInvoiceLineDto> Lines
    );
}
