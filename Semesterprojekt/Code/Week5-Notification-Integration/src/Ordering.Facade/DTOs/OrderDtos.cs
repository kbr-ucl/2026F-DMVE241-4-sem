namespace Ordering.Facade.DTOs;
public record PlaceOrderRequest(Guid EventId, string CustomerEmail, List<OrderLineRequest> Lines);
public record OrderLineRequest(Guid CategoryId, int Quantity);
public record OrderDto(Guid Id, Guid EventId, string CustomerEmail, string Status,
    decimal TotalAmount, DateTime CreatedAt, List<OrderLineDto> Lines);
public record OrderLineDto(Guid CategoryId, string CategoryName, int Quantity, decimal UnitPrice);
public record GetOrderRequest(Guid OrderId);
