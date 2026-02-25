using EventCatalog.Facade.DTOs;
using EventCatalog.Facade.Interfaces;
using EventCatalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EventCatalog.Infrastructure.Queries;

/// <summary>
/// Query-handler: Implementerer Facade-interface direkte (CQS).
/// AsNoTracking() for optimal læseperformance.
/// </summary>
public class EventQueriesImpl : IEventQueries
{
    private readonly EventCatalogDbContext _db;
    public EventQueriesImpl(EventCatalogDbContext db) => _db = db;

    public async Task<EventDto?> GetByIdAsync(GetEventRequest request)
        => await _db.Events.AsNoTracking().Include(e => e.TicketCategories)
            .Where(e => e.Id == request.EventId)
            .Select(e => new EventDto(e.Id, e.Name, e.Description, e.Date, e.Venue,
                e.Status.ToString(),
                e.TicketCategories.Select(c => new TicketCategoryDto(c.Id, c.Name, c.Price.Amount, c.TotalCapacity)).ToList()))
            .FirstOrDefaultAsync();

    public async Task<IReadOnlyList<EventDto>> SearchAsync(SearchEventsRequest request)
    {
        var q = _db.Events.AsNoTracking().Include(e => e.TicketCategories).AsQueryable();
        if (!string.IsNullOrWhiteSpace(request.NameFilter))
            q = q.Where(e => e.Name.Contains(request.NameFilter));
        if (request.FromDate.HasValue)
            q = q.Where(e => e.Date >= request.FromDate.Value);
        return await q.Select(e => new EventDto(e.Id, e.Name, e.Description, e.Date, e.Venue,
            e.Status.ToString(),
            e.TicketCategories.Select(c => new TicketCategoryDto(c.Id, c.Name, c.Price.Amount, c.TotalCapacity)).ToList()))
            .ToListAsync();
    }
}
