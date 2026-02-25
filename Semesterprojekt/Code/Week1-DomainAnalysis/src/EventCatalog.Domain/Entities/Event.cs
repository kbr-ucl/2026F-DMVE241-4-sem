using EventCatalog.Domain.Exceptions;

namespace EventCatalog.Domain.Entities;

/// <summary>
///     Aggregate Root: Et event med billetkategorier.
///     Alle tilstandsændringer sker via metoder der håndhæver forretningsregler.
///     Ingen public setters – DDD Entity-mønster.
/// </summary>
public class Event
{
    private List<TicketCategory> _ticketCategories = new();
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime Date { get; private set; }
    public string Venue { get; private set; } = string.Empty;
    public EventStatus Status { get; private set; }

    public IReadOnlyList<TicketCategory> TicketCategories
    {
        get => _ticketCategories.AsReadOnly();
        private set => _ticketCategories = value.ToList();
    }

    private Event()
    {
    }

    public Event(string name, string description, DateTime date, string venue)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new DomainException("Event name is required.");
        if (date <= DateTime.UtcNow)
            throw new DomainException("Event date must be in the future.");

        Id = Guid.NewGuid();
        Name = name;
        Description = description;
        Date = DateTime.SpecifyKind(date, DateTimeKind.Utc);
        Venue = venue;
        Status = EventStatus.Draft;
    }

    public void AddTicketCategory(string name, ValueObjects.Money price, int capacity)
    {
        if (Status == EventStatus.Cancelled)
            throw new DomainException("Cannot modify a cancelled event.");

        if (_ticketCategories.Any(tc => tc.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            throw new DomainException($"Ticket category '{name}' already exists.");

        _ticketCategories.Add(new TicketCategory(this, name, price, capacity));
    }

    public void Publish()
    {
        if (_ticketCategories.Count == 0)
            throw new DomainException("Cannot publish without ticket categories.");
        if (Status != EventStatus.Draft)
            throw new DomainException("Only draft events can be published.");
        Status = EventStatus.Published;
    }

    public void Cancel()
    {
        if (Status == EventStatus.Cancelled)
            throw new DomainException("Event is already cancelled.");
        Status = EventStatus.Cancelled;
    }

    public void MarkAlmostSoldOut()
    {
        if (Status != EventStatus.Published)
            throw new DomainException("Only published events can be marked.");
        Status = EventStatus.AlmostSoldOut;
    }
}