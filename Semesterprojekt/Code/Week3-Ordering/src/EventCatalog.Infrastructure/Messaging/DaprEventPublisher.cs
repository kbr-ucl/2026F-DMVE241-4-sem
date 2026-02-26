using Dapr.Client;
using EventCatalog.UseCases.Ports;

namespace EventCatalog.Infrastructure.Messaging;

public class DaprEventPublisher : IEventPublisher
{
    private readonly DaprClient _dapr;
    public DaprEventPublisher(DaprClient dapr) => _dapr = dapr;

    public async Task PublishEventCreatedAsync(Guid eventId, string eventName, DateTime eventDate,
        List<(Guid CategoryId, string Name, int Capacity)> categories)
    {
        await _dapr.PublishEventAsync("tickethub-pubsub", "event-created", new
        {
            EventId = eventId, EventName = eventName, EventDate = eventDate,
            Categories = categories.Select(c => new { c.CategoryId, CategoryName = c.Name, c.Capacity })
        });
    }
}
