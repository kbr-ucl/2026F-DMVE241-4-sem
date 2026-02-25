namespace EventCatalog.Facade.DTOs;

// === Command Request DTOs ===
public record CreateEventRequest(string Name, string Description, DateTime Date,
    string Venue, List<TicketCategoryRequest> TicketCategories);
public record TicketCategoryRequest(string Name, decimal Price, int Capacity);
public record PublishEventRequest(Guid EventId);

// === Query Request DTOs ===
public record GetEventRequest(Guid EventId);
public record SearchEventsRequest(string? NameFilter, DateTime? FromDate);

// === Response DTOs ===
public record EventDto(Guid Id, string Name, string Description, DateTime Date,
    string Venue, string Status, List<TicketCategoryDto> TicketCategories);
public record TicketCategoryDto(Guid Id, string Name, decimal Price, int TotalCapacity);
