namespace Inventory.Domain.Entities;

/// <summary>
/// Entity: Lagerbeholdning per billetkategori.
/// "TicketCategory" har her en anden betydning end i EventCatalog (Ubiquitous Language).
/// </summary>
public class TicketStock
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public Guid CategoryId { get; private set; }
    public string CategoryName { get; private set; } = string.Empty;
    public int TotalCapacity { get; private set; }
    public int Available { get; private set; }
    public int Reserved { get; private set; }
    public byte[] RowVersion { get; private set; } = [];

    private TicketStock() { }

    public TicketStock(Guid eventId, Guid categoryId, string categoryName, int totalCapacity)
    {
        if (totalCapacity <= 0) throw new DomainException("Capacity must be > 0.");
        Id = Guid.NewGuid(); EventId = eventId; CategoryId = categoryId;
        CategoryName = categoryName; TotalCapacity = totalCapacity;
        Available = totalCapacity; Reserved = 0;
    }

    public void Reserve(int qty)
    {
        if (qty <= 0) throw new DomainException("Quantity must be positive.");
        if (qty > Available) throw new DomainException($"Only {Available} available.");
        Available -= qty; Reserved += qty;
    }

    public void Release(int qty)
    {
        if (qty <= 0) throw new DomainException("Quantity must be positive.");
        if (qty > Reserved) throw new DomainException($"Only {Reserved} reserved.");
        Reserved -= qty; Available += qty;
    }

    public void ConfirmSale(int qty)
    {
        if (qty > Reserved) throw new DomainException("Cannot confirm more than reserved.");
        Reserved -= qty;
    }

    public bool IsLow(double threshold = 0.1)
        => TotalCapacity > 0 && (double)Available / TotalCapacity <= threshold;
}
