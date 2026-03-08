using Xunit;
using Ordering.Domain.ValueObjects;
using Ordering.Domain;

namespace Ordering.Domain.Tests;

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
    public void Money_Addition_ReturnsCorrectResult()
    {
        // Arrange
        var money1 = Money.FromDecimal(100m);
        var money2 = Money.FromDecimal(50m);

        // Act
        var result = money1 + money2;

        // Assert
        Assert.Equal(150m, result.Amount);
    }

    [Fact]
    public void Money_Multiplication_ReturnsCorrectResult()
    {
        // Arrange
        var money = Money.FromDecimal(10m);
        var multiplier = 5;

        // Act
        var result = money * multiplier;

        // Assert
        Assert.Equal(50m, result.Amount);
    }

    [Fact]
    public void Money_Subtraction_WhenResultPositive_ReturnsCorrectResult()
    {
        // Arrange
        var money1 = Money.FromDecimal(100m);
        var money2 = Money.FromDecimal(30m);

        // Act
        var result = money1 - money2;

        // Assert
        Assert.Equal(70m, result.Amount);
    }

    [Fact]
    public void Money_Subtraction_WhenResultZeroOrNegative_ThrowsDomainException()
    {
        // Arrange
        var money1 = Money.FromDecimal(50m);
        var money2 = Money.FromDecimal(100m);

        // Act & Assert
        Assert.Throws<DomainException>(() => money1 - money2);
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

    [Fact]
    public void Money_Equality_WhenDifferentAmount_ReturnsFalse()
    {
        // Arrange
        var money1 = Money.FromDecimal(100m);
        var money2 = Money.FromDecimal(200m);

        // Act
        var areEqual = money1.Equals(money2);

        // Assert
        Assert.False(areEqual);
    }

    [Fact]
    public void Money_ToString_ReturnsFormattedAmount()
    {
        // Arrange
        var money = Money.FromDecimal(123.456m);

        // Act
        var result = money.ToString();

        // Assert
        Assert.Equal("123.46", result);
    }
}
