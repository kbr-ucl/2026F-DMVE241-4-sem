using Xunit;
using EventCatalog.Domain.Entities;
using EventCatalog.Domain.Exceptions;
using EventCatalog.Domain.ValueObjects;

namespace EventCatalog.Domain.Tests;

public class EventTests
{
    private static Event CreateValidEvent()
        => new("Roskilde Festival", "Musikfestival", DateTime.UtcNow.AddDays(30), "Roskilde");

    [Fact]
    public void Constructor_ValidInput_CreatesEventWithDraftStatus()
    {
        // Act
        var sut = CreateValidEvent();

        // Assert
        Assert.NotEqual(Guid.Empty, sut.Id);
        Assert.Equal("Roskilde Festival", sut.Name);
        Assert.Equal(EventStatus.Draft, sut.Status);
        Assert.Empty(sut.TicketCategories);
    }

    [Fact]
    public void Constructor_EmptyName_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(
            () => new Event("", "Desc", DateTime.UtcNow.AddDays(1), "Venue"));
        Assert.Contains("name", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Constructor_PastDate_ThrowsDomainException()
    {
        // Act & Assert
        var ex = Assert.Throws<DomainException>(
            () => new Event("Test", "Desc", DateTime.UtcNow.AddDays(-1), "Venue"));
        Assert.Contains("future", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AddTicketCategory_ValidInput_AddsCategory()
    {
        // Arrange
        var sut = CreateValidEvent();

        // Act
        sut.AddTicketCategory("Standard", Money.FromDecimal(500m), 100);

        // Assert
        Assert.Single(sut.TicketCategories);
        Assert.Equal("Standard", sut.TicketCategories[0].Name);
    }

    [Fact]
    public void AddTicketCategory_DuplicateName_ThrowsDomainException()
    {
        // Arrange
        var sut = CreateValidEvent();
        sut.AddTicketCategory("VIP", Money.FromDecimal(1000m), 50);

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => sut.AddTicketCategory("vip", Money.FromDecimal(1200m), 30));
        Assert.Contains("already exists", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void AddTicketCategory_CancelledEvent_ThrowsDomainException()
    {
        // Arrange
        var sut = CreateValidEvent();
        sut.Cancel();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => sut.AddTicketCategory("Standard", Money.FromDecimal(500m), 100));
        Assert.Contains("cancelled", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Publish_WithCategories_SetsStatusToPublished()
    {
        // Arrange
        var sut = CreateValidEvent();
        sut.AddTicketCategory("Standard", Money.FromDecimal(500m), 100);

        // Act
        sut.Publish();

        // Assert
        Assert.Equal(EventStatus.Published, sut.Status);
    }

    [Fact]
    public void Publish_WithoutCategories_ThrowsDomainException()
    {
        // Arrange
        var sut = CreateValidEvent();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => sut.Publish());
        Assert.Contains("categories", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Publish_AlreadyPublished_ThrowsDomainException()
    {
        // Arrange
        var sut = CreateValidEvent();
        sut.AddTicketCategory("Standard", Money.FromDecimal(500m), 100);
        sut.Publish();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => sut.Publish());
        Assert.Contains("draft", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void Cancel_DraftEvent_SetsStatusToCancelled()
    {
        // Arrange
        var sut = CreateValidEvent();

        // Act
        sut.Cancel();

        // Assert
        Assert.Equal(EventStatus.Cancelled, sut.Status);
    }

    [Fact]
    public void Cancel_AlreadyCancelled_ThrowsDomainException()
    {
        // Arrange
        var sut = CreateValidEvent();
        sut.Cancel();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => sut.Cancel());
        Assert.Contains("cancelled", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void MarkAlmostSoldOut_PublishedEvent_SetsStatus()
    {
        // Arrange
        var sut = CreateValidEvent();
        sut.AddTicketCategory("Standard", Money.FromDecimal(500m), 100);
        sut.Publish();

        // Act
        sut.MarkAlmostSoldOut();

        // Assert
        Assert.Equal(EventStatus.AlmostSoldOut, sut.Status);
    }

    [Fact]
    public void MarkAlmostSoldOut_DraftEvent_ThrowsDomainException()
    {
        // Arrange
        var sut = CreateValidEvent();

        // Act & Assert
        var ex = Assert.Throws<DomainException>(() => sut.MarkAlmostSoldOut());
        Assert.Contains("published", ex.Message, StringComparison.OrdinalIgnoreCase);
    }
}
