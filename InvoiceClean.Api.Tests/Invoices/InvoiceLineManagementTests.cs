using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace InvoiceClean.Api.Tests.Invoices;

public sealed class InvoiceLineManagementTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public InvoiceLineManagementTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task AddLine_to_existing_invoice_succeeds()
    {
        // Arrange - Create invoice with one line
        var createRequest = new
        {
            number = "INV-LINE-001",
            date = "2026-01-15",
            lines = new[]
            {
                new { description = "Initial Service", quantity = 1, unitPrice = 100m }
            }
        };

        var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
        var invoiceId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        // Act - Add new line
        var addLineRequest = new
        {
            description = "Additional Service",
            quantity = 2,
            unitPrice = 50m
        };

        var addResponse = await _client.PostAsJsonAsync(
            $"/api/invoices/{invoiceId}/lines", addLineRequest);

        // Assert
        addResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var invoice = await addResponse.Content.ReadFromJsonAsync<InvoiceDto>();
        invoice.Should().NotBeNull();
        invoice!.Lines.Should().HaveCount(2);
        invoice.Total.Should().Be(200m); // 100 + (2 * 50)
        
        var newLine = invoice.Lines.FirstOrDefault(l => l.Description == "Additional Service");
        newLine.Should().NotBeNull();
        newLine!.Quantity.Should().Be(2);
        newLine.UnitPrice.Should().Be(50m);
        newLine.LineTotal.Should().Be(100m);
    }

    [Fact]
    public async Task AddLine_to_non_existing_invoice_returns_not_found()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        var addLineRequest = new
        {
            description = "Service",
            quantity = 1,
            unitPrice = 100m
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            $"/api/invoices/{nonExistingId}/lines", addLineRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("invoice_not_found");
    }

    [Fact]
    public async Task AddLine_with_invalid_data_returns_validation_error()
    {
        // Arrange - Create invoice
        var createRequest = new
        {
            number = "INV-LINE-002",
            date = "2026-01-15",
            lines = new[] { new { description = "Service", quantity = 1, unitPrice = 100m } }
        };

        var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
        var invoiceId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        // Act - Add line with invalid quantity
        var addLineRequest = new
        {
            description = "Invalid Service",
            quantity = 0, // Invalid - must be > 0
            unitPrice = 100m
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/invoices/{invoiceId}/lines", addLineRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Quantity must be greater than 0");
    }

    [Fact]
    public async Task UpdateLine_modifies_existing_line_successfully()
    {
        // Arrange - Create invoice with line
        var createRequest = new
        {
            number = "INV-LINE-003",
            date = "2026-01-15",
            lines = new[]
            {
                new { description = "Original Service", quantity = 1, unitPrice = 100m }
            }
        };

        var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
        var invoiceId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        var getResponse = await _client.GetAsync($"/api/invoices/{invoiceId}");
        var originalInvoice = await getResponse.Content.ReadFromJsonAsync<InvoiceDto>();
        var lineId = originalInvoice!.Lines[0].Id;

        // Act - Update the line
        var updateLineRequest = new
        {
            description = "Updated Service",
            quantity = 3,
            unitPrice = 75m
        };

        var updateResponse = await _client.PutAsJsonAsync(
            $"/api/invoices/{invoiceId}/lines/{lineId}", updateLineRequest);

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedInvoice = await updateResponse.Content.ReadFromJsonAsync<InvoiceDto>();
        updatedInvoice!.Lines.Should().HaveCount(1);
        updatedInvoice.Lines[0].Description.Should().Be("Updated Service");
        updatedInvoice.Lines[0].Quantity.Should().Be(3);
        updatedInvoice.Lines[0].UnitPrice.Should().Be(75m);
        updatedInvoice.Lines[0].LineTotal.Should().Be(225m);
        updatedInvoice.Total.Should().Be(225m);
    }

    [Fact]
    public async Task UpdateLine_with_non_existing_line_id_returns_error()
    {
        // Arrange - Create invoice
        var createRequest = new
        {
            number = "INV-LINE-004",
            date = "2026-01-15",
            lines = new[] { new { description = "Service", quantity = 1, unitPrice = 100m } }
        };

        var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
        var invoiceId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        // Act - Try to update non-existing line
        var nonExistingLineId = Guid.NewGuid();
        var updateLineRequest = new
        {
            description = "Updated Service",
            quantity = 1,
            unitPrice = 50m
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/invoices/{invoiceId}/lines/{nonExistingLineId}", updateLineRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Invoice line with ID");
        content.Should().Contain("not found");
    }

    [Fact]
    public async Task RemoveLine_deletes_line_from_invoice()
    {
        // Arrange - Create invoice with two lines
        var createRequest = new
        {
            number = "INV-LINE-005",
            date = "2026-01-15",
            lines = new[]
            {
                new { description = "Service A", quantity = 1, unitPrice = 100m },
                new { description = "Service B", quantity = 2, unitPrice = 50m }
            }
        };

        var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
        var invoiceId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        var getResponse = await _client.GetAsync($"/api/invoices/{invoiceId}");
        var originalInvoice = await getResponse.Content.ReadFromJsonAsync<InvoiceDto>();
        var lineToRemove = originalInvoice!.Lines.First(l => l.Description == "Service B");

        // Act - Remove Service B line
        var deleteResponse = await _client.DeleteAsync(
            $"/api/invoices/{invoiceId}/lines/{lineToRemove.Id}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updatedInvoice = await deleteResponse.Content.ReadFromJsonAsync<InvoiceDto>();
        updatedInvoice!.Lines.Should().HaveCount(1);
        updatedInvoice.Lines.Should().Contain(l => l.Description == "Service A");
        updatedInvoice.Total.Should().Be(100m);
    }

    [Fact]
    public async Task RemoveLine_last_line_returns_domain_error()
    {
        // Arrange - Create invoice with one line
        var createRequest = new
        {
            number = "INV-LINE-006",
            date = "2026-01-15",
            lines = new[]
            {
                new { description = "Only Service", quantity = 1, unitPrice = 100m }
            }
        };

        var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
        var invoiceId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        var getResponse = await _client.GetAsync($"/api/invoices/{invoiceId}");
        var invoice = await getResponse.Content.ReadFromJsonAsync<InvoiceDto>();
        var lineId = invoice!.Lines[0].Id;

        // Act - Try to remove the only line
        var deleteResponse = await _client.DeleteAsync(
            $"/api/invoices/{invoiceId}/lines/{lineId}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await deleteResponse.Content.ReadAsStringAsync();
        content.Should().Contain("Cannot remove the last invoice line");
        content.Should().Contain("must have at least one line");
    }

    [Fact]
    public async Task RemoveLine_with_non_existing_line_id_returns_error()
    {
        // Arrange - Create invoice
        var createRequest = new
        {
            number = "INV-LINE-007",
            date = "2026-01-15",
            lines = new[]
            {
                new { description = "Service A", quantity = 1, unitPrice = 100m },
                new { description = "Service B", quantity = 1, unitPrice = 50m }
            }
        };

        var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
        var invoiceId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        // Act - Try to remove non-existing line
        var nonExistingLineId = Guid.NewGuid();
        var deleteResponse = await _client.DeleteAsync(
            $"/api/invoices/{invoiceId}/lines/{nonExistingLineId}");

        // Assert
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await deleteResponse.Content.ReadAsStringAsync();
        content.Should().Contain("Invoice line with ID");
        content.Should().Contain("not found");
    }

    [Fact]
    public async Task Multiple_line_operations_maintain_invoice_consistency()
    {
        // Arrange - Create invoice
        var createRequest = new
        {
            number = "INV-LINE-008",
            date = "2026-01-15",
            lines = new[]
            {
                new { description = "Service A", quantity = 1, unitPrice = 100m }
            }
        };

        var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
        var invoiceId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        // Act - Add line
        await _client.PostAsJsonAsync($"/api/invoices/{invoiceId}/lines", 
            new { description = "Service B", quantity = 2, unitPrice = 50m });

        // Act - Add another line
        await _client.PostAsJsonAsync($"/api/invoices/{invoiceId}/lines", 
            new { description = "Service C", quantity = 3, unitPrice = 25m });

        // Get current state
        var getResponse1 = await _client.GetAsync($"/api/invoices/{invoiceId}");
        var invoice1 = await getResponse1.Content.ReadFromJsonAsync<InvoiceDto>();
        
        var lineServiceA = invoice1!.Lines.First(l => l.Description == "Service A");
        var lineServiceB = invoice1.Lines.First(l => l.Description == "Service B");

        // Act - Update Service A line
        await _client.PutAsJsonAsync($"/api/invoices/{invoiceId}/lines/{lineServiceA.Id}",
            new { description = "Service A Updated", quantity = 5, unitPrice = 30m });

        // Act - Remove Service B line
        await _client.DeleteAsync($"/api/invoices/{invoiceId}/lines/{lineServiceB.Id}");

        // Assert - Final state
        var finalResponse = await _client.GetAsync($"/api/invoices/{invoiceId}");
        var finalInvoice = await finalResponse.Content.ReadFromJsonAsync<InvoiceDto>();

        finalInvoice!.Lines.Should().HaveCount(2);
        
        var updatedLineA = finalInvoice.Lines.First(l => l.Description == "Service A Updated");
        updatedLineA.LineTotal.Should().Be(150m); // 5 * 30
        
        var lineC = finalInvoice.Lines.First(l => l.Description == "Service C");
        lineC.LineTotal.Should().Be(75m); // 3 * 25
        
        finalInvoice.Total.Should().Be(225m); // 150 + 75
    }

    [Fact]
    public async Task AddLine_with_negative_unit_price_returns_error()
    {
        // Arrange
        var createRequest = new
        {
            number = "INV-LINE-009",
            date = "2026-01-15",
            lines = new[] { new { description = "Service", quantity = 1, unitPrice = 100m } }
        };

        var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
        var invoiceId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        // Act - Add line with negative price
        var addLineRequest = new
        {
            description = "Invalid Service",
            quantity = 1,
            unitPrice = -50m // Invalid
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/invoices/{invoiceId}/lines", addLineRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Unit price must be greater than or equal to 0");
    }

    [Fact]
    public async Task UpdateLine_with_empty_description_returns_error()
    {
        // Arrange
        var createRequest = new
        {
            number = "INV-LINE-010",
            date = "2026-01-15",
            lines = new[] { new { description = "Original Service", quantity = 1, unitPrice = 100m } }
        };

        var postResponse = await _client.PostAsJsonAsync("/api/invoices", createRequest);
        var invoiceId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        var getResponse = await _client.GetAsync($"/api/invoices/{invoiceId}");
        var invoice = await getResponse.Content.ReadFromJsonAsync<InvoiceDto>();
        var lineId = invoice!.Lines[0].Id;

        // Act - Update with empty description
        var updateLineRequest = new
        {
            description = "", // Invalid
            quantity = 1,
            unitPrice = 100m
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/invoices/{invoiceId}/lines/{lineId}", updateLineRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Description is required");
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