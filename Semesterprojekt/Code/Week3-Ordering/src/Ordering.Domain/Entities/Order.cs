using Ordering.Domain.ValueObjects;

namespace Ordering.Domain.Entities;
public class Order
{
    public Guid Id { get; private set; }
    public Guid EventId { get; private set; }
    public CustomerEmail CustomerEmail { get; private set; } = null!;
    public OrderStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public string? CancellationReason { get; private set; }
    private readonly List<OrderLine> _lines = new();
    public IReadOnlyList<OrderLine> Lines => _lines.AsReadOnly();
    private Order() { }
    public Order(Guid eventId, CustomerEmail customerEmail, List<OrderLine> lines)
    {
        if (customerEmail == null) throw new DomainException("Email required.");
        if (lines.Count == 0) throw new DomainException("Order must have lines.");
        Id = Guid.NewGuid(); EventId = eventId; CustomerEmail = customerEmail;
        Status = OrderStatus.Created; CreatedAt = DateTime.UtcNow; _lines.AddRange(lines);
    }
    public void Confirm()
    {
        if (Status != OrderStatus.Created) throw new DomainException("Only created orders can be confirmed.");
        Status = OrderStatus.Confirmed; ConfirmedAt = DateTime.UtcNow;
    }
    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Cancelled) throw new DomainException("Already cancelled.");
        Status = OrderStatus.Cancelled; CancelledAt = DateTime.UtcNow; CancellationReason = reason;
    }
    public Money TotalAmount
    {
        get
        {
            var total = _lines.Sum(l => l.UnitPrice.Amount * l.Quantity);
            return Money.FromDecimal(total);
        }
    }
}
