using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace InvoiceClean.Api.Tests.Invoices
{
    public sealed class InvoiceEndpointsTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public InvoiceEndpointsTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Post_then_get_invoice_returns_full_invoice()
        {
            // Arrange
            var createRequest = new
            {
                number = "INV-TEST-001",
                date = "2025-12-21",
                lines = new[]
                {
                new { description = "Service A", quantity = 2, unitPrice = 50m }
            }
            };

            // Act — POST
            var postResponse = await _client.PostAsJsonAsync(
                "/api/invoices", createRequest);

            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var id = await postResponse.Content.ReadFromJsonAsync<Guid>();
            id.Should().NotBeEmpty();

            // Act — GET
            var getResponse = await _client.GetAsync($"/api/invoices/{id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var invoice = await getResponse.Content.ReadFromJsonAsync<InvoiceDto>();

            // Assert
            invoice!.Number.Should().Be("INV-TEST-001");
            invoice.Total.Should().Be(100m);
            invoice.Lines.Should().HaveCount(1);
            invoice.Lines[0].LineTotal.Should().Be(100m);
        }

        private sealed record InvoiceDto(
            Guid Id,
            string Number,
            DateOnly Date,
            decimal Total,
            IReadOnlyList<InvoiceLineDto> Lines
        );

        private sealed record InvoiceLineDto(
            Guid Id,
            string Description,
            decimal Quantity,
            decimal UnitPrice,
            decimal LineTotal
        );
    }
}
