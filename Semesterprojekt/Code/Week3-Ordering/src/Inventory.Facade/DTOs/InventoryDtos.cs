namespace Inventory.Facade.DTOs;

public record ReserveTicketsRequest(Guid OrderId, Guid EventId, List<ReservationLine> Lines);
public record ReservationLine(Guid CategoryId, int Quantity);
public record ReleaseTicketsRequest(Guid OrderId, Guid EventId, List<ReservationLine> Lines);
public record TicketStockDto(Guid CategoryId, string CategoryName, int Available, int Reserved, int TotalCapacity);
public record GetStockRequest(Guid EventId);
