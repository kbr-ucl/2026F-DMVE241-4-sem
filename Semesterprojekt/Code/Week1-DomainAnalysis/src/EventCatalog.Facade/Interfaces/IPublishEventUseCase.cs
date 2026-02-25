using EventCatalog.Facade.DTOs;

namespace EventCatalog.Facade.Interfaces;

/// <summary>Command: Publicerer et event og udsender EventCreated-event.</summary>
public interface IPublishEventUseCase
{
    Task Execute(PublishEventRequest request);
}