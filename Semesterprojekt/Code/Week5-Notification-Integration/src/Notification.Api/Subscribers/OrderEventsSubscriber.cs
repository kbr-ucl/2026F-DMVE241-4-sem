using Dapr;
using Microsoft.AspNetCore.Mvc;
namespace Notification.Api.Subscribers;

/// <summary>
/// Lytter på OrderConfirmed og OrderCancelled events.
/// Notifikation simuleres via logging (ren event-drevet service).
/// </summary>
[ApiController]
public class OrderEventsSubscriber(ILogger<OrderEventsSubscriber> logger) : ControllerBase
{
    [Topic("tickethub-pubsub", "order-confirmed")]
    [HttpPost("/subscribe/order-confirmed")]
    public IActionResult OnConfirmed(OrderConfirmedEvent evt)
    {
        logger.LogInformation("NOTIFICATION: Order {OrderId} confirmed for {Email}", evt.OrderId, evt.CustomerEmail);
        return Ok();
    }

    [Topic("tickethub-pubsub", "order-cancelled")]
    [HttpPost("/subscribe/order-cancelled")]
    public IActionResult OnCancelled(OrderCancelledEvent evt)
    {
        logger.LogInformation("NOTIFICATION: Order {OrderId} cancelled for {Email}. Reason: {Reason}", evt.OrderId, evt.CustomerEmail, evt.Reason);
        return Ok();
    }
}
public record OrderConfirmedEvent(Guid OrderId, Guid EventId, string CustomerEmail);
public record OrderCancelledEvent(Guid OrderId, Guid EventId, string CustomerEmail, string Reason);
