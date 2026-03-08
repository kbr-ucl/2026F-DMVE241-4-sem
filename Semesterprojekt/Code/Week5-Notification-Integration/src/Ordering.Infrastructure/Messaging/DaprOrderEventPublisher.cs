using Dapr.Client;
using Ordering.UseCases.Ports;
namespace Ordering.Infrastructure.Messaging;
public class DaprOrderEventPublisher : IOrderEventPublisher
{
    private readonly DaprClient _dapr;
    public DaprOrderEventPublisher(DaprClient dapr) => _dapr = dapr;
    public async Task PublishOrderConfirmedAsync(Guid orderId, Guid eventId, string email)
        => await _dapr.PublishEventAsync("tickethub-pubsub", "order-confirmed", new { OrderId = orderId, EventId = eventId, CustomerEmail = email });
    public async Task PublishOrderCancelledAsync(Guid orderId, Guid eventId, string email, string reason)
        => await _dapr.PublishEventAsync("tickethub-pubsub", "order-cancelled", new { OrderId = orderId, EventId = eventId, CustomerEmail = email, Reason = reason });
}
