using Xunit;
using EventCatalog.Domain;
using EventCatalog.Domain.Entities;
using EventCatalog.Domain.ValueObjects;

namespace EventCatalog.Domain.Tests;

public class TicketCategoryTests
{
    private static Event CreateValidEvent()
        => new("Test Event", "Beskrivelse", DateTime.UtcNow.AddDays(30), "Odense");

    [Fact]
    public void Constructor_ValidInput_CreatesCategory()
    {
        // Arrange
        var ev = CreateValidEvent();

        // Act
        var sut = new TicketCategory(ev, "Standard", Money.FromDecimal(500m), 100);

        // Assert
        Assert.NotEqual(Guid.Empty, sut.Id);
        Assert.Equal("Standard", sut.Name);
        Assert.Equal(500m, sut.Price.Amount);
        Assert.Equal(100, sut.TotalCapacity);
        Assert.Same(ev, sut.Event);
    }

    [Fact]
    public void Constructor_EmptyName_ThrowsDomainException()
    {
        // Arrange
        var ev = CreateValidEvent();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new TicketCategory(ev, "", Money.FromDecimal(500m), 100));
        Assert.Contains("name", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_NegativePrice_ThrowsDomainException()
    {
        // Arrange
        var ev = CreateValidEvent();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => Money.FromDecimal(-1m));
        Assert.Contains("positive", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_ZeroCapacity_ThrowsDomainException()
    {
        // Arrange
        var ev = CreateValidEvent();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new TicketCategory(ev, "Standard", Money.FromDecimal(500m), 0));
        Assert.Contains("capacity", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_NegativeCapacity_ThrowsDomainException()
    {
        // Arrange
        var ev = CreateValidEvent();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => new TicketCategory(ev, "Standard", Money.FromDecimal(500m), -5));
        Assert.Contains("capacity", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UpdatePrice_ValidPrice_UpdatesPrice()
    {
        // Arrange
        var ev = CreateValidEvent();
        var sut = new TicketCategory(ev, "Standard", Money.FromDecimal(500m), 100);

        // Act
        sut.UpdatePrice(Money.FromDecimal(750m));

        // Assert
        Assert.Equal(750m, sut.Price.Amount);
    }

    [Fact]
    public void UpdatePrice_ZeroPrice_ThrowsDomainException()
    {
        // Arrange
        var ev = CreateValidEvent();
        var sut = new TicketCategory(ev, "Standard", Money.FromDecimal(500m), 100);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => Money.FromDecimal(0m));
        Assert.Contains("positive", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void UpdatePrice_NegativePrice_ThrowsDomainException()
    {
        // Arrange
        var ev = CreateValidEvent();
        var sut = new TicketCategory(ev, "Standard", Money.FromDecimal(500m), 100);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => Money.FromDecimal(-1m));
        Assert.Contains("positive", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
