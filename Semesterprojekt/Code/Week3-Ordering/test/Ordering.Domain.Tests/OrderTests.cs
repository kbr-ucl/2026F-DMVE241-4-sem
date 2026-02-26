using Xunit;
using Ordering.Domain;
using Ordering.Domain.Entities;
using Ordering.Domain.ValueObjects;

namespace Ordering.Domain.Tests;

public class OrderTests
{
    private static List<OrderLine> CreateValidLines()
        => [new OrderLine(Guid.NewGuid(), "Standard", 2, Money.FromDecimal(500m))];

    [Fact]
    public void Constructor_ValidInput_CreatesOrderWithCreatedStatus()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        var lines = CreateValidLines();

        // Act
        var sut = new Order(eventId, CustomerEmail.From("test@example.com"), lines);

        // Assert
        Assert.NotEqual(Guid.Empty, sut.Id);
        Assert.Equal(eventId, sut.EventId);
        Assert.Equal("test@example.com", sut.CustomerEmail.Value);
        Assert.Equal(OrderStatus.Created, sut.Status);
        Assert.Single(sut.Lines);
        Assert.Null(sut.ConfirmedAt);
        Assert.Null(sut.CancelledAt);
        Assert.Null(sut.CancellationReason);
    }

    [Fact]
    public void Constructor_EmptyEmail_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(
            () => new Order(Guid.NewGuid(), CustomerEmail.From(""), CreateValidLines()));
        Assert.Contains("email", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_EmptyLines_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(
            () => new Order(Guid.NewGuid(), CustomerEmail.From("test@example.com"), []));
        Assert.Contains("lines", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void TotalAmount_SingleLine_CalculatesCorrectly()
    {
        // Arrange
        var lines = new List<OrderLine>
        {
            new(Guid.NewGuid(), "Standard", 3, Money.FromDecimal(500m))
        };
        var sut = new Order(Guid.NewGuid(), CustomerEmail.From("test@example.com"), lines);

        // Act & Assert
        Assert.Equal(1500m, sut.TotalAmount.Amount);
    }

    [Fact]
    public void TotalAmount_MultipleLines_CalculatesCorrectly()
    {
        // Arrange
        var lines = new List<OrderLine>
        {
            new(Guid.NewGuid(), "Standard", 2, Money.FromDecimal(500m)),
            new(Guid.NewGuid(), "VIP", 1, Money.FromDecimal(2000m))
        };
        var sut = new Order(Guid.NewGuid(), CustomerEmail.From("test@example.com"), lines);

        // Act & Assert
        Assert.Equal(3000m, sut.TotalAmount.Amount);
    }

    [Fact]
    public void Confirm_CreatedOrder_SetsStatusToConfirmed()
    {
        // Arrange
        var sut = new Order(Guid.NewGuid(), CustomerEmail.From("test@example.com"), CreateValidLines());

        // Act
        sut.Confirm();

        // Assert
        Assert.Equal(OrderStatus.Confirmed, sut.Status);
        Assert.NotNull(sut.ConfirmedAt);
    }

    [Fact]
    public void Confirm_AlreadyConfirmed_ThrowsDomainException()
    {
        // Arrange
        var sut = new Order(Guid.NewGuid(), CustomerEmail.From("test@example.com"), CreateValidLines());
        sut.Confirm();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => sut.Confirm());
        Assert.Contains("created", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Confirm_CancelledOrder_ThrowsDomainException()
    {
        // Arrange
        var sut = new Order(Guid.NewGuid(), CustomerEmail.From("test@example.com"), CreateValidLines());
        sut.Cancel("Test");

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => sut.Confirm());
        Assert.Contains("created", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Cancel_CreatedOrder_SetsStatusToCancelled()
    {
        // Arrange
        var sut = new Order(Guid.NewGuid(), CustomerEmail.From("test@example.com"), CreateValidLines());

        // Act
        sut.Cancel("Kunde fortroed");

        // Assert
        Assert.Equal(OrderStatus.Cancelled, sut.Status);
        Assert.NotNull(sut.CancelledAt);
        Assert.Equal("Kunde fortroed", sut.CancellationReason);
    }

    [Fact]
    public void Cancel_AlreadyCancelled_ThrowsDomainException()
    {
        // Arrange
        var sut = new Order(Guid.NewGuid(), CustomerEmail.From("test@example.com"), CreateValidLines());
        sut.Cancel("Foerste gang");

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => sut.Cancel("Anden gang"));
        Assert.Contains("cancelled", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Cancel_ConfirmedOrder_Succeeds()
    {
        // Arrange
        var sut = new Order(Guid.NewGuid(), CustomerEmail.From("test@example.com"), CreateValidLines());
        sut.Confirm();

        // Act
        sut.Cancel("Refund requested");

        // Assert
        Assert.Equal(OrderStatus.Cancelled, sut.Status);
    }
}
