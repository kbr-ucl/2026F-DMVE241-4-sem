using Xunit;
using Inventory.Domain;
using Inventory.Domain.Entities;

namespace Inventory.Domain.Tests;

public class TicketStockTests
{
    private static TicketStock CreateValidStock(int capacity = 100)
        => new(Guid.NewGuid(), Guid.NewGuid(), "Standard", capacity);

    [Fact]
    public void Constructor_ValidInput_CreatesStock()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();

        // Act
        var sut = new TicketStock(eventId, categoryId, "VIP", 200);

        // Assert
        Assert.NotEqual(Guid.Empty, sut.Id);
        Assert.Equal(eventId, sut.EventId);
        Assert.Equal(categoryId, sut.CategoryId);
        Assert.Equal("VIP", sut.CategoryName);
        Assert.Equal(200, sut.TotalCapacity);
        Assert.Equal(200, sut.Available);
        Assert.Equal(0, sut.Reserved);
    }

    [Fact]
    public void Constructor_ZeroCapacity_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(
            () => new TicketStock(Guid.NewGuid(), Guid.NewGuid(), "Standard", 0));
    }

    [Fact]
    public void Constructor_NegativeCapacity_ThrowsDomainException()
    {
        // Act & Assert
        Assert.Throws<DomainException>(
            () => new TicketStock(Guid.NewGuid(), Guid.NewGuid(), "Standard", -5));
    }

    [Fact]
    public void Reserve_ValidQuantity_DecreasesAvailableIncreasesReserved()
    {
        // Arrange
        var sut = CreateValidStock(100);

        // Act
        sut.Reserve(10);

        // Assert
        Assert.Equal(90, sut.Available);
        Assert.Equal(10, sut.Reserved);
    }

    [Fact]
    public void Reserve_AllAvailable_Succeeds()
    {
        // Arrange
        var sut = CreateValidStock(50);

        // Act
        sut.Reserve(50);

        // Assert
        Assert.Equal(0, sut.Available);
        Assert.Equal(50, sut.Reserved);
    }

    [Fact]
    public void Reserve_MoreThanAvailable_ThrowsDomainException()
    {
        // Arrange
        var sut = CreateValidStock(10);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => sut.Reserve(11));
        Assert.Contains("available", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Reserve_ZeroQuantity_ThrowsDomainException()
    {
        // Arrange
        var sut = CreateValidStock();

        // Act & Assert
        Assert.Throws<DomainException>(() => sut.Reserve(0));
    }

    [Fact]
    public void Reserve_NegativeQuantity_ThrowsDomainException()
    {
        // Arrange
        var sut = CreateValidStock();

        // Act & Assert
        Assert.Throws<DomainException>(() => sut.Reserve(-1));
    }

    [Fact]
    public void Release_ValidQuantity_IncreasesAvailableDecreasesReserved()
    {
        // Arrange
        var sut = CreateValidStock(100);
        sut.Reserve(20);

        // Act
        sut.Release(10);

        // Assert
        Assert.Equal(90, sut.Available);
        Assert.Equal(10, sut.Reserved);
    }

    [Fact]
    public void Release_AllReserved_Succeeds()
    {
        // Arrange
        var sut = CreateValidStock(100);
        sut.Reserve(30);

        // Act
        sut.Release(30);

        // Assert
        Assert.Equal(100, sut.Available);
        Assert.Equal(0, sut.Reserved);
    }

    [Fact]
    public void Release_MoreThanReserved_ThrowsDomainException()
    {
        // Arrange
        var sut = CreateValidStock(100);
        sut.Reserve(5);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => sut.Release(10));
        Assert.Contains("reserved", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Release_ZeroQuantity_ThrowsDomainException()
    {
        // Arrange
        var sut = CreateValidStock();

        // Act & Assert
        Assert.Throws<DomainException>(() => sut.Release(0));
    }

    [Fact]
    public void ConfirmSale_ValidQuantity_DecreasesReserved()
    {
        // Arrange
        var sut = CreateValidStock(100);
        sut.Reserve(20);

        // Act
        sut.ConfirmSale(10);

        // Assert
        Assert.Equal(80, sut.Available);
        Assert.Equal(10, sut.Reserved);
    }

    [Fact]
    public void ConfirmSale_MoreThanReserved_ThrowsDomainException()
    {
        // Arrange
        var sut = CreateValidStock(100);
        sut.Reserve(5);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => sut.ConfirmSale(10));
        Assert.Contains("reserved", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void IsLow_BelowThreshold_ReturnsTrue()
    {
        // Arrange
        var sut = CreateValidStock(100);
        sut.Reserve(95); // 5 available = 5%

        // Act & Assert
        Assert.True(sut.IsLow(0.1));
    }

    [Fact]
    public void IsLow_AboveThreshold_ReturnsFalse()
    {
        // Arrange
        var sut = CreateValidStock(100);
        sut.Reserve(50); // 50 available = 50%

        // Act & Assert
        Assert.False(sut.IsLow(0.1));
    }

    [Fact]
    public void IsLow_ExactlyAtThreshold_ReturnsTrue()
    {
        // Arrange
        var sut = CreateValidStock(100);
        sut.Reserve(90); // 10 available = 10%

        // Act & Assert
        Assert.True(sut.IsLow(0.1));
    }

    [Fact]
    public void Reserve_Release_MaintainsConsistency()
    {
        // Arrange
        var sut = CreateValidStock(100);

        // Act
        sut.Reserve(30);
        sut.Release(10);
        sut.Reserve(5);

        // Assert
        Assert.Equal(75, sut.Available);
        Assert.Equal(25, sut.Reserved);
        Assert.Equal(100, sut.Available + sut.Reserved);
    }
}
