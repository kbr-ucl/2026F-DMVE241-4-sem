namespace Ordering.UseCases.Ports;
public interface IInventoryService
{
    Task<bool> ReserveTicketsAsync(Guid orderId, Guid eventId, List<(Guid CategoryId, int Quantity)> lines);
    Task ReleaseTicketsAsync(Guid orderId, Guid eventId, List<(Guid CategoryId, int Quantity)> lines);
}
