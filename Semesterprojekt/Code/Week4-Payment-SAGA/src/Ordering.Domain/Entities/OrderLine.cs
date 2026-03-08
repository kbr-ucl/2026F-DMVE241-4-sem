using Ordering.Domain.ValueObjects;

namespace Ordering.Domain.Entities;
public class OrderLine
{
    public Guid Id { get; private set; }
    public Guid CategoryId { get; private set; }
    public string CategoryName { get; private set; } = string.Empty;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = null!;
    private OrderLine() { }
    public OrderLine(Guid categoryId, string categoryName, int quantity, Money unitPrice)
    {
        if (quantity <= 0) throw new DomainException("Quantity must be positive.");
        if (unitPrice == null) throw new DomainException("Unit price is required.");
        Id = Guid.NewGuid(); CategoryId = categoryId; CategoryName = categoryName;
        Quantity = quantity; UnitPrice = unitPrice;
    }
}
