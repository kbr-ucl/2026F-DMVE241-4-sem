using Xunit;
using Ordering.Domain;
using Ordering.Domain.Entities;
using Ordering.Domain.ValueObjects;

namespace Ordering.Domain.Tests;

public class OrderLineTests
{
    [Fact]
    public void Constructor_ValidInput_CreatesOrderLine()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act
        var sut = new OrderLine(categoryId, "VIP", 3, Money.FromDecimal(1500m));

        // Assert
        Assert.NotEqual(Guid.Empty, sut.Id);
        Assert.Equal(categoryId, sut.CategoryId);
        Assert.Equal("VIP", sut.CategoryName);
        Assert.Equal(3, sut.Quantity);
        Assert.Equal(1500m, sut.UnitPrice.Amount);
    }

    [Fact]
    public void Constructor_ZeroQuantity_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(
            () => new OrderLine(Guid.NewGuid(), "Standard", 0, Money.FromDecimal(500m)));
        Assert.Contains("quantity", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_NegativeQuantity_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(
            () => new OrderLine(Guid.NewGuid(), "Standard", -1, Money.FromDecimal(500m)));
        Assert.Contains("quantity", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_NegativePrice_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(
            () => Money.FromDecimal(-100m));
        Assert.Contains("positive", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_ZeroPrice_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(
            () => Money.FromDecimal(0m));
        Assert.Contains("positive", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
