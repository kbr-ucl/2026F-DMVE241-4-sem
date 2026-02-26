using Xunit;
using EventCatalog.Domain.ValueObjects;
using EventCatalog.Domain;

namespace EventCatalog.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void Money_WhenNegative_ThrowsDomainException()
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() => Money.FromDecimal(-10m));
    }

    [Fact]
    public void Money_WhenZero_ThrowsDomainException()
    {
        // Arrange & Act & Assert
        Assert.Throws<DomainException>(() => Money.FromDecimal(0m));
    }

    [Fact]
    public void Money_WhenPositive_CreatesSuccessfully()
    {
        // Arrange
        var amount = 100.50m;

        // Act
        var money = Money.FromDecimal(amount);

        // Assert
        Assert.Equal(amount, money.Amount);
    }

    [Fact]
    public void Money_Equality_WhenSameAmount_ReturnsTrue()
    {
        // Arrange
        var money1 = Money.FromDecimal(100m);
        var money2 = Money.FromDecimal(100m);

        // Act
        var areEqual = money1.Equals(money2);

        // Assert
        Assert.True(areEqual);
    }
}
