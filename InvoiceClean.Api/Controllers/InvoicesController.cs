using InvoiceClean.Application.Invoices.CreateInvoice;
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
        public async Task<ActionResult<Guid>> Create(CreateInvoiceRequest request, CancellationToken ct)
        {
            var id = await _mediator.Send(
                new CreateInvoiceCommand(request.Number, request.Date, request.Lines), ct);

            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult> GetById(Guid id, CancellationToken ct)
        {
            // (sljedeći korak: Query + response DTO)
            return Ok(new { id });
        }
    }

    public sealed record CreateInvoiceRequest(
        string Number,
        DateOnly Date,
        List<CreateInvoiceLineDto> Lines
    );
}
