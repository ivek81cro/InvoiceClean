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

        [Fact]
        public async Task Update_invoice_updates_header_data()
        {
            // Arrange - Create invoice first
            var createRequest = new
            {
                number = "INV-TEST-002",
                date = "2025-12-21",
                lines = new[]
                {
                    new { description = "Service B", quantity = 1, unitPrice = 100m }
                }
            };

            var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
            var id = await postResponse.Content.ReadFromJsonAsync<Guid>();

            // Act - Update invoice
            var updateRequest = new
            {
                number = "INV-UPDATED-002",
                date = "2025-12-25",
                customerName = "Test Customer",
                customerAddress = "123 Test Street",
                customerVat = "VAT123456"
            };

            var putResponse = await _client.PutAsJsonAsync($"/api/invoices/{id}", updateRequest);

            // Assert
            putResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedInvoice = await putResponse.Content.ReadFromJsonAsync<InvoiceDto>();
            updatedInvoice!.Number.Should().Be("INV-UPDATED-002");
            updatedInvoice.Date.Should().Be(new DateOnly(2025, 12, 25));
            updatedInvoice.Total.Should().Be(100m); // Total should remain unchanged
            updatedInvoice.Lines.Should().HaveCount(1); // Lines should remain unchanged
        }

        [Fact]
        public async Task Update_non_existing_invoice_returns_not_found()
        {
            // Arrange
            var nonExistingId = Guid.NewGuid();
            var updateRequest = new
            {
                number = "INV-999",
                date = "2025-12-21",
                customerName = "Test Customer",
                customerAddress = (string?)null,
                customerVat = (string?)null
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/invoices/{nonExistingId}", updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_invoice_with_customer_data_persists_changes()
        {
            // Arrange - Create invoice
            var createRequest = new
            {
                number = "INV-TEST-003",
                date = "2025-12-21",
                lines = new[]
                {
                    new { description = "Service C", quantity = 2, unitPrice = 75m }
                }
            };

            var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
            var id = await postResponse.Content.ReadFromJsonAsync<Guid>();

            // Act - Update with customer data
            var updateRequest = new
            {
                number = "INV-TEST-003",
                date = "2025-12-21",
                customerName = "Acme Corporation",
                customerAddress = "456 Business Ave, Zagreb",
                customerVat = "HR12345678901"
            };

            await _client.PutAsJsonAsync($"/api/invoices/{id}", updateRequest);

            // Assert - Verify changes persisted
            var getResponse = await _client.GetAsync($"/api/invoices/{id}");
            var invoice = await getResponse.Content.ReadFromJsonAsync<InvoiceDto>();

            invoice!.Number.Should().Be("INV-TEST-003");
            invoice.Total.Should().Be(150m);
            invoice.Lines.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetAll_returns_list_of_invoice_ids()
        {
            // Arrange - Create multiple invoices
            var request1 = new
            {
                number = "INV-LIST-001",
                date = "2025-12-20",
                lines = new[] { new { description = "Item 1", quantity = 1, unitPrice = 50m } }
            };

            var request2 = new
            {
                number = "INV-LIST-002",
                date = "2025-12-21",
                lines = new[] { new { description = "Item 2", quantity = 1, unitPrice = 75m } }
            };

            await _client.PostAsJsonAsync("/api/invoices", request1);
            await _client.PostAsJsonAsync("/api/invoices", request2);

            // Act
            var response = await _client.GetAsync("/api/invoices");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var invoices = await response.Content.ReadFromJsonAsync<List<InvoiceIdDto>>();
            invoices.Should().NotBeNull();
            invoices.Should().HaveCountGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task Update_invoice_with_invalid_data_returns_validation_error()
        {
            // Arrange - Create invoice first
            var createRequest = new
            {
                number = "INV-TEST-004",
                date = "2025-12-21",
                lines = new[]
                {
                    new { description = "Service D", quantity = 1, unitPrice = 50m }
                }
            };

            var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
            var id = await postResponse.Content.ReadFromJsonAsync<Guid>();

            // Act - Update with invalid data (empty customer name)
            var updateRequest = new
            {
                number = "INV-TEST-004",
                date = "2025-12-21",
                customerName = "",  // Invalid - empty
                customerAddress = (string?)null,
                customerVat = (string?)null
            };

            var response = await _client.PutAsJsonAsync($"/api/invoices/{id}", updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            var content = await response.Content.ReadAsStringAsync();
            content.Should().Contain("Customer name is required");
        }

        [Fact]
        public async Task Update_invoice_with_too_long_number_returns_validation_error()
        {
            // Arrange
            var createRequest = new
            {
                number = "INV-TEST-005",
                date = "2025-12-21",
                lines = new[]
                {
                    new { description = "Service E", quantity = 1, unitPrice = 50m }
                }
            };

            var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
            var id = await postResponse.Content.ReadFromJsonAsync<Guid>();

            // Act - Update with too long number
            var updateRequest = new
            {
                number = new string('X', 51),  // 51 characters - exceeds max length
                date = "2025-12-21",
                customerName = "Test Customer",
                customerAddress = (string?)null,
                customerVat = (string?)null
            };

            var response = await _client.PutAsJsonAsync($"/api/invoices/{id}", updateRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

        private sealed record InvoiceIdDto(
            Guid Id,
            string Number,
            DateOnly Date
        );
    }
}
