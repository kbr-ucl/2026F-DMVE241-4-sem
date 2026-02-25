using EventCatalog.Domain.Exceptions;
using EventCatalog.Domain.ValueObjects;

namespace EventCatalog.Domain.Entities;

/// <summary>
///     Entity: En billetkategori (f.eks. "Standard", "VIP") med pris og kapacitet.
///     Private setters + metoder = DDD-princip.
/// </summary>
public class TicketCategory
{
    public Guid Id { get; private set; }
    public Event Event { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public Money Price { get; private set; } = null!;
    public int TotalCapacity { get; private set; }

    private TicketCategory()
    {
    }

    public TicketCategory(Event @event, string name, Money price, int totalCapacity)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Category name is required.");
        if (price == null)
            throw new DomainException("Price is required.");
        if (totalCapacity <= 0)
            throw new DomainException("Capacity must be greater than zero.");

        Id = Guid.NewGuid();
        Event = @event;
        Name = name;
        Price = price;
        TotalCapacity = totalCapacity;
    }

    public void UpdatePrice(Money newPrice)
    {
        if (newPrice == null) throw new DomainException("Price is required.");
        Price = newPrice;
    }
}