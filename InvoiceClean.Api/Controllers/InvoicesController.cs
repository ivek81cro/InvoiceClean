using InvoiceClean.Api.Common;
using InvoiceClean.Api.Contracts;
using InvoiceClean.Application.Invoices.CreateInvoice;
using InvoiceClean.Application.Invoices.GetAllInvoiceId;
using InvoiceClean.Application.Invoices.GetInvoiceById;
using InvoiceClean.Application.Invoices.UpdateInvoice;
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

        [HttpGet]
        public async Task<ActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllInvoiceIdsQuery(), cancellationToken);
            return this.ToActionResult(result);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateInvoice(Guid id, [FromBody] UpdateInvoiceRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateInvoiceCommand(
                id,
                request.Number,
                request.Date,
                request.CustomerName,
                request.CustomerAddress,
                request.CustomerVat
            );

            var result = await _mediator.Send(command, cancellationToken);

            return this.ToActionResult(result);
        }
    }

    public sealed record CreateInvoiceRequest(
        string Number,
        DateOnly Date,
        List<CreateInvoiceLineDto> Lines
    );
}
