using EventCatalog.Facade.DTOs;
using EventCatalog.Facade.Interfaces;
using EventCatalog.UseCases.Ports;
using EventCatalog.UseCases.Repositories;

namespace EventCatalog.UseCases.Commands;

/// <summary>
/// Command: Publicerer event og sender integration event via pub/sub.
/// </summary>
public class PublishEventUseCase : IPublishEventUseCase
{
    private readonly IEventRepository _repo;
    private readonly IEventPublisher _publisher;

    public PublishEventUseCase(IEventRepository repo, IEventPublisher publisher)
    { _repo = repo; _publisher = publisher; }

    public async Task Execute(PublishEventRequest request)
    {
        var evt = await _repo.GetByIdAsync(request.EventId)
            ?? throw new InvalidOperationException("Event not found.");

        evt.Publish(); // Forretningsregel i domain-entity
        await _repo.SaveAsync();

        // Integration event via Dapr pub/sub
        await _publisher.PublishEventCreatedAsync(
            evt.Id, evt.Name, evt.Date,
            evt.TicketCategories.Select(c => (c.Id, c.Name, c.TotalCapacity)).ToList());
    }
}
