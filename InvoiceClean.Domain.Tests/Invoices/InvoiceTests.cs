using FluentAssertions;
using InvoiceClean.Domain.Invoices;

namespace InvoiceClean.Domain.Tests.Invoices;

public sealed class InvoiceTests
{
    [Fact]
    public void UpdateLine_with_valid_data_succeeds()
    {
        // Arrange
        var invoice = new Invoice("INV-001", new DateOnly(2026, 1, 15));
        invoice.AddLine("Service A", 1, 100m);
        var lineId = invoice.Lines.First().Id;

        // Act
        invoice.UpdateLine(lineId, "Service A Updated", 2, 75m);

        // Assert
        var line = invoice.Lines.First();
        line.Description.Should().Be("Service A Updated");
        line.Quantity.Should().Be(2);
        line.UnitPrice.Should().Be(75m);
        line.LineTotal.Should().Be(150m);
        invoice.Total.Should().Be(150m);
    }

    [Fact]
    public void UpdateLine_with_non_existing_id_throws_exception()
    {
        // Arrange
        var invoice = new Invoice("INV-001", new DateOnly(2026, 1, 15));
        invoice.AddLine("Service A", 1, 100m);
        var nonExistingId = Guid.NewGuid();

        // Act
        var act = () => invoice.UpdateLine(nonExistingId, "Updated", 1, 100m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage($"Invoice line with ID '{nonExistingId}' not found.");
    }

    [Fact]
    public void RemoveLine_with_multiple_lines_succeeds()
    {
        // Arrange
        var invoice = new Invoice("INV-001", new DateOnly(2026, 1, 15));
        invoice.AddLine("Service A", 1, 100m);
        invoice.AddLine("Service B", 2, 50m);
        var lineToRemove = invoice.Lines.Last().Id;

        // Act
        invoice.RemoveLine(lineToRemove);

        // Assert
        invoice.Lines.Should().HaveCount(1);
        invoice.Lines.First().Description.Should().Be("Service A");
        invoice.Total.Should().Be(100m);
    }

    [Fact]
    public void RemoveLine_last_line_throws_exception()
    {
        // Arrange
        var invoice = new Invoice("INV-001", new DateOnly(2026, 1, 15));
        invoice.AddLine("Service A", 1, 100m);
        var lineId = invoice.Lines.First().Id;

        // Act
        var act = () => invoice.RemoveLine(lineId);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot remove the last invoice line. Invoice must have at least one line.");
    }

    [Fact]
    public void RemoveLine_with_non_existing_id_throws_exception()
    {
        // Arrange
        var invoice = new Invoice("INV-001", new DateOnly(2026, 1, 15));
        invoice.AddLine("Service A", 1, 100m);
        invoice.AddLine("Service B", 1, 50m);
        var nonExistingId = Guid.NewGuid();

        // Act
        var act = () => invoice.RemoveLine(nonExistingId);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage($"Invoice line with ID '{nonExistingId}' not found.");
    }

    [Fact]
    public void InvoiceLine_Update_with_valid_data_succeeds()
    {
        // Arrange
        var line = new InvoiceLine("Original", 1, 100m);

        // Act
        line.Update("Updated", 3, 50m);

        // Assert
        line.Description.Should().Be("Updated");
        line.Quantity.Should().Be(3);
        line.UnitPrice.Should().Be(50m);
        line.LineTotal.Should().Be(150m);
    }

    [Fact]
    public void InvoiceLine_Update_with_empty_description_throws_exception()
    {
        // Arrange
        var line = new InvoiceLine("Original", 1, 100m);

        // Act
        var act = () => line.Update("", 1, 100m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Description is required.");
    }

    [Fact]
    public void InvoiceLine_Update_with_zero_quantity_throws_exception()
    {
        // Arrange
        var line = new InvoiceLine("Original", 1, 100m);

        // Act
        var act = () => line.Update("Valid", 0, 100m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Quantity must be > 0.");
    }

    [Fact]
    public void InvoiceLine_Update_with_negative_price_throws_exception()
    {
        // Arrange
        var line = new InvoiceLine("Original", 1, 100m);

        // Act
        var act = () => line.Update("Valid", 1, -10m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Unit price must be >= 0.");
    }
}