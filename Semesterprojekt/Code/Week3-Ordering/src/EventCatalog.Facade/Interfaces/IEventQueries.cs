using EventCatalog.Facade.DTOs;
namespace EventCatalog.Facade.Interfaces;

/// <summary>
/// Query-interface: Læseoperationer. Implementeres i Infrastructure direkte (CQS).
/// </summary>
public interface IEventQueries
{
    Task<EventDto?> GetByIdAsync(GetEventRequest request);
    Task<IReadOnlyList<EventDto>> SearchAsync(SearchEventsRequest request);
}
