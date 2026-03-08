namespace EventCatalog.UseCases.Ports;

/// <summary>Port: Publicering af integration events. Impl. i Infrastructure med Dapr.</summary>
public interface IEventPublisher
{
    Task PublishEventCreatedAsync(Guid eventId, string eventName, DateTime eventDate,
        List<(Guid CategoryId, string Name, int Capacity)> categories);
}
