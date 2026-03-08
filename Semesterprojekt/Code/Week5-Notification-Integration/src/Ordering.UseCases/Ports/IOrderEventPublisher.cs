namespace Ordering.UseCases.Ports;
public interface IOrderEventPublisher
{
    Task PublishOrderConfirmedAsync(Guid orderId, Guid eventId, string customerEmail);
    Task PublishOrderCancelledAsync(Guid orderId, Guid eventId, string customerEmail, string reason);
}
